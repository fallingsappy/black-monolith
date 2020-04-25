using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.BE.Enums.Options;
using DDrop.BE.Models;
using DDrop.Controls.PixelDrawer;
using Brushes = System.Windows.Media.Brushes;

namespace DDrop.BL.GeometryBL
{
    public class GeometryBL : IGeometryBL
    {
        public void PrepareLines(BE.Models.DropPhoto selectedPhoto, out Line horizontalLine, out Line verticalLine,
            bool showLinesOnPreview)
        {
            if (selectedPhoto.HorizontalLine != null && showLinesOnPreview)
                horizontalLine = new Line
                {
                    X1 = selectedPhoto.HorizontalLine.X1,
                    X2 = selectedPhoto.HorizontalLine.X2,
                    Y1 = selectedPhoto.HorizontalLine.Y1,
                    Y2 = selectedPhoto.HorizontalLine.Y2,
                    StrokeThickness = 2,
                    Stroke = Brushes.DeepPink
                };
            else
                horizontalLine = null;

            if (selectedPhoto.VerticalLine != null && showLinesOnPreview)
                verticalLine = new Line
                {
                    X1 = selectedPhoto.VerticalLine.X1,
                    X2 = selectedPhoto.VerticalLine.X2,
                    Y1 = selectedPhoto.VerticalLine.Y1,
                    Y2 = selectedPhoto.VerticalLine.Y2,
                    StrokeThickness = 2,
                    Stroke = Brushes.Green
                };
            else
                verticalLine = null;
        }

        public void CreateDiameters(BE.Models.DropPhoto dropPhoto, Point[] points)
        {
            var biggestHorizontalDistance = 0;
            var biggestVerticalDistance = 0;
            var simpleHorizontalDiameter = new SimpleLine();
            var simpleVerticalDiameter = new SimpleLine();

            for (var i = 0; i < points.Length; i++)
            for (var j = i + 1; j < points.Length - 1; j++)
            {
                var currentHorizontalDistance = Math.Abs(points[i].X - points[j].X);
                if (currentHorizontalDistance > biggestHorizontalDistance)
                {
                    biggestHorizontalDistance = currentHorizontalDistance;

                    simpleHorizontalDiameter.X1 = points[i].X;
                    simpleHorizontalDiameter.Y1 = points[i].Y;
                    simpleHorizontalDiameter.SimpleLineId = dropPhoto.SimpleHorizontalLineId ?? Guid.NewGuid();
                    simpleHorizontalDiameter.X2 = points[j].X;
                    simpleHorizontalDiameter.Y2 = points[j].Y;
                }

                var currentVerticalDistance = Math.Abs(points[i].Y - points[j].Y);
                if (currentVerticalDistance > biggestVerticalDistance)
                {
                    biggestVerticalDistance = currentVerticalDistance;

                    simpleVerticalDiameter.X1 = points[i].X;
                    simpleVerticalDiameter.Y1 = points[i].Y;
                    simpleVerticalDiameter.SimpleLineId = dropPhoto.SimpleVerticalLineId ?? Guid.NewGuid();
                    simpleVerticalDiameter.X2 = points[j].X;
                    simpleVerticalDiameter.Y2 = points[j].Y;
                }
            }

            if (dropPhoto.SimpleHorizontalLine == null)
                dropPhoto.SimpleHorizontalLine = new SimpleLine();

            dropPhoto.SimpleHorizontalLine.X1 = simpleHorizontalDiameter.X1;
            dropPhoto.SimpleHorizontalLine.X2 = simpleHorizontalDiameter.X2;
            dropPhoto.SimpleHorizontalLine.Y1 = simpleHorizontalDiameter.Y1;
            dropPhoto.SimpleHorizontalLine.Y2 = simpleHorizontalDiameter.Y1;
            dropPhoto.SimpleHorizontalLine.SimpleLineId = simpleHorizontalDiameter.SimpleLineId;

            if (dropPhoto.HorizontalLine == null)
                dropPhoto.HorizontalLine = new Line();

            dropPhoto.HorizontalLine.X1 = simpleHorizontalDiameter.X1;
            dropPhoto.HorizontalLine.X2 = simpleHorizontalDiameter.X2;
            dropPhoto.HorizontalLine.Y1 = simpleHorizontalDiameter.Y1;
            dropPhoto.HorizontalLine.Y2 = simpleHorizontalDiameter.Y1;
            dropPhoto.HorizontalLine.StrokeThickness = 2;
            dropPhoto.HorizontalLine.Stroke = Brushes.DeepPink;

            if (dropPhoto.SimpleVerticalLine == null)
                dropPhoto.SimpleVerticalLine = new SimpleLine();

            dropPhoto.SimpleVerticalLine.X1 = simpleVerticalDiameter.X1;
            dropPhoto.SimpleVerticalLine.X2 = simpleVerticalDiameter.X1;
            dropPhoto.SimpleVerticalLine.Y1 = simpleVerticalDiameter.Y1;
            dropPhoto.SimpleVerticalLine.Y2 = simpleVerticalDiameter.Y2;
            dropPhoto.SimpleVerticalLine.SimpleLineId = simpleVerticalDiameter.SimpleLineId;

            if (dropPhoto.VerticalLine == null)
                dropPhoto.VerticalLine = new Line();

            dropPhoto.VerticalLine.X1 = simpleVerticalDiameter.X1;
            dropPhoto.VerticalLine.X2 = simpleVerticalDiameter.X1;
            dropPhoto.VerticalLine.Y1 = simpleVerticalDiameter.Y1;
            dropPhoto.VerticalLine.Y2 = simpleVerticalDiameter.Y2;
            dropPhoto.VerticalLine.StrokeThickness = 2;
            dropPhoto.VerticalLine.Stroke = Brushes.Green;
        }

        public void RestoreOriginalLines(BE.Models.DropPhoto dropPhoto, BE.Models.DropPhoto storedPhoto, Canvas canvas)
        {
            if (storedPhoto.SimpleHorizontalLine != null)
            {
                dropPhoto.SimpleHorizontalLine.X1 = storedPhoto.SimpleHorizontalLine.X1;
                dropPhoto.SimpleHorizontalLine.X2 = storedPhoto.SimpleHorizontalLine.X2;
                dropPhoto.SimpleHorizontalLine.Y1 = storedPhoto.SimpleHorizontalLine.Y1;
                dropPhoto.SimpleHorizontalLine.Y2 = storedPhoto.SimpleHorizontalLine.Y2;
                dropPhoto.SimpleHorizontalLine.SimpleLineId = storedPhoto.SimpleHorizontalLine.SimpleLineId;

                dropPhoto.HorizontalLine.X1 = dropPhoto.SimpleHorizontalLine.X1;
                dropPhoto.HorizontalLine.X2 = dropPhoto.SimpleHorizontalLine.X2;
                dropPhoto.HorizontalLine.Y1 = dropPhoto.SimpleHorizontalLine.Y1;
                dropPhoto.HorizontalLine.Y2 = dropPhoto.SimpleHorizontalLine.Y2;
            }
            else
            {
                canvas.Children.Remove(dropPhoto.HorizontalLine);
                dropPhoto.SimpleHorizontalLine = null;
                dropPhoto.HorizontalLine = null;
            }

            if (storedPhoto.SimpleVerticalLine != null)
            {
                dropPhoto.SimpleVerticalLine.X1 = storedPhoto.SimpleVerticalLine.X1;
                dropPhoto.SimpleVerticalLine.X2 = storedPhoto.SimpleVerticalLine.X2;
                dropPhoto.SimpleVerticalLine.Y1 = storedPhoto.SimpleVerticalLine.Y1;
                dropPhoto.SimpleVerticalLine.Y2 = storedPhoto.SimpleVerticalLine.Y2;
                dropPhoto.SimpleVerticalLine.SimpleLineId = storedPhoto.SimpleVerticalLine.SimpleLineId;

                dropPhoto.VerticalLine.X1 = dropPhoto.SimpleVerticalLine.X1;
                dropPhoto.VerticalLine.X2 = dropPhoto.SimpleVerticalLine.X2;
                dropPhoto.VerticalLine.Y1 = dropPhoto.SimpleVerticalLine.Y1;
                dropPhoto.VerticalLine.Y2 = dropPhoto.SimpleVerticalLine.Y2;
            }
            else
            {
                canvas.Children.Remove(dropPhoto.VerticalLine);
                dropPhoto.SimpleVerticalLine = null;
                dropPhoto.VerticalLine = null;
            }
        }

        public void PrepareContour(BE.Models.DropPhoto selectedPhoto, out ObservableCollection<Line> contour,
            bool showContourOnPreview)
        {
            if (selectedPhoto.Contour?.Lines != null && showContourOnPreview)
            {
                contour = new ObservableCollection<Line>();

                foreach (var item in selectedPhoto.Contour.Lines)
                {
                    var lineForAdd = new Line
                    {
                        X1 = item.X1,
                        X2 = item.X2,
                        Y1 = item.Y1,
                        Y2 = item.Y2,
                        Stroke = item.Stroke,
                        Fill = item.Fill
                    };

                    contour.Add(lineForAdd);
                }
            }
            else
            {
                contour = null;
            }
        }

        public void CreateContour(BE.Models.DropPhoto dropPhoto, Point[] points,
            CalculationVariants calculationVariant, string cShrpKsize, string cShrpSize1, string cShrpSize2,
            string cShrpThreshold1, string cShrpThreshold2, string pythonKSise, string pythonSize1, string pythonSize2,
            string pythonThreshold1,
            string pythonThreshold2, BE.Models.DropPhoto currentDropPhoto, PixelDrawer imgCurrent)
        {
            if (dropPhoto.Contour == null)
            {
                dropPhoto.Contour = new Contour
                {
                    ContourId = dropPhoto.DropPhotoId,
                    CurrentDropPhoto = dropPhoto,
                    SimpleLines = new ObservableCollection<SimpleLine>(),
                    Lines = new ObservableCollection<Line>()
                };
            }
            else
            {
                if (dropPhoto.Contour != null && dropPhoto == currentDropPhoto)
                    foreach (var line in dropPhoto.Contour.Lines)
                        imgCurrent.CanDrawing.Children.Remove(line);

                dropPhoto.Contour.SimpleLines.Clear();
                dropPhoto.Contour.Lines.Clear();
            }

            dropPhoto.Contour.CalculationVariants = calculationVariant;

            if (calculationVariant == CalculationVariants.CalculateWithCSharp)
                dropPhoto.Contour.Parameters = new AutoCalculationParameters
                {
                    Ksize = Convert.ToInt32(cShrpKsize),
                    Size1 = Convert.ToInt32(cShrpSize1),
                    Size2 = Convert.ToInt32(cShrpSize2),
                    Treshold1 = Convert.ToInt32(cShrpThreshold1),
                    Treshold2 = Convert.ToInt32(cShrpThreshold2)
                };
            else
                dropPhoto.Contour.Parameters = new AutoCalculationParameters
                {
                    Ksize = Convert.ToInt32(pythonKSise),
                    Size1 = Convert.ToInt32(pythonSize1),
                    Size2 = Convert.ToInt32(pythonSize2),
                    Treshold1 = Convert.ToInt32(pythonThreshold1),
                    Treshold2 = Convert.ToInt32(pythonThreshold2)
                };

            for (var j = 0; j < points.Length; j++)
            {
                dropPhoto.Contour.SimpleLines.Add(new SimpleLine
                {
                    ContourId = dropPhoto.Contour.ContourId,
                    SimpleLineId = Guid.NewGuid(),
                    X1 = points[j].X,
                    X2 = j < points.Length - 1 ? points[j + 1].X : points[0].X,
                    Y1 = points[j].Y,
                    Y2 = j < points.Length - 1 ? points[j + 1].Y : points[0].Y
                });

                dropPhoto.Contour.Lines.Add(new Line
                {
                    X1 = points[j].X,
                    X2 = j < points.Length - 1 ? points[j + 1].X : points[0].X,
                    Y1 = points[j].Y,
                    Y2 = j < points.Length - 1 ? points[j + 1].Y : points[0].Y,
                    StrokeThickness = 2,
                    Stroke = Brushes.Red
                });
            }
        }

        public void StoreContour(BE.Models.DropPhoto dropPhoto, BE.Models.DropPhoto storeTo)
        {
            if (dropPhoto.Contour != null)
            {
                var storedContour = new Contour
                {
                    ContourId = dropPhoto.DropPhotoId,
                    CalculationVariants = dropPhoto.Contour.CalculationVariants,
                    Parameters = new AutoCalculationParameters
                    {
                        Ksize = dropPhoto.Contour.Parameters.Ksize,
                        Size1 = dropPhoto.Contour.Parameters.Size1,
                        Size2 = dropPhoto.Contour.Parameters.Size2,
                        Treshold1 = dropPhoto.Contour.Parameters.Treshold1,
                        Treshold2 = dropPhoto.Contour.Parameters.Treshold2
                    },
                    SimpleLines = new ObservableCollection<SimpleLine>()
                };

                foreach (var line in dropPhoto.Contour.SimpleLines)
                    storedContour.SimpleLines.Add(new SimpleLine
                    {
                        X1 = line.X1,
                        X2 = line.X2,
                        Y1 = line.Y1,
                        Y2 = line.Y2,
                        SimpleLineId = line.SimpleLineId,
                        ContourId = line.ContourId
                    });

                storeTo.Contour = storedContour;
            }
            else
            {
                storeTo.Contour = null;
            }
        }

        public void RestoreOriginalContour(BE.Models.DropPhoto dropPhoto, BE.Models.DropPhoto storedPhoto,
            Canvas canvas, Guid currentDropPhotoId)
        {
            if (storedPhoto.Contour != null && dropPhoto.Contour != null)
            {
                dropPhoto.Contour.ContourId = storedPhoto.Contour.ContourId;
                dropPhoto.Contour.CalculationVariants = storedPhoto.Contour.CalculationVariants;
                dropPhoto.Contour.Parameters = storedPhoto.Contour.Parameters;
                dropPhoto.Contour.SimpleLines = storedPhoto.Contour.SimpleLines;
            }
            else if (storedPhoto.Contour != null && dropPhoto.Contour == null)
            {
                dropPhoto.Contour = new Contour
                {
                    SimpleLines = new ObservableCollection<SimpleLine>(),
                    ContourId = storedPhoto.Contour.ContourId,
                    CalculationVariants = storedPhoto.Contour.CalculationVariants,
                    Parameters = storedPhoto.Contour.Parameters
                };

                dropPhoto.Contour.SimpleLines = storedPhoto.Contour.SimpleLines;
            }
            else
            {
                if (dropPhoto.Contour != null && dropPhoto.DropPhotoId == currentDropPhotoId)
                    foreach (var line in dropPhoto.Contour.Lines)
                        canvas.Children.Remove(line);

                dropPhoto.Contour = null;
            }
        }
    }
}