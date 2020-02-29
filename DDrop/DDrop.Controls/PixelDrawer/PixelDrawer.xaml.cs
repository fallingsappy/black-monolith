using DDrop.BE.Models;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;

namespace DDrop.Controls.PixelDrawer
{
    /// <summary>
    /// Interaction logic for PixelDrawer.xaml
    /// </summary>
    public partial class PixelDrawer
    {
        public static readonly DependencyProperty PixelsInMillimeterProperty = DependencyProperty.Register("PixelsInMillimeter", typeof(string), typeof(PixelDrawer));
        public static readonly DependencyProperty PixelsInMillimeterVerticalProperty = DependencyProperty.Register("PixelsInMillimeterVertical", typeof(string), typeof(PixelDrawer));
        public static readonly DependencyProperty PixelsInMillimeterHorizontalProperty = DependencyProperty.Register("PixelsInMillimeterHorizontal", typeof(string), typeof(PixelDrawer));
        public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(PixelDrawer));
        public static readonly DependencyProperty CurrentSeriesProperty = DependencyProperty.Register("CurrentSeries", typeof(Series), typeof(PixelDrawer));
        public static readonly DependencyProperty CurrentDropPhotoProperty = DependencyProperty.Register("CurrentDropPhoto", typeof(DropPhoto), typeof(PixelDrawer));

        public DropPhoto CurrentDropPhoto
        {
            get { return (DropPhoto)GetValue(CurrentDropPhotoProperty); }
            set
            {
                SetValue(CurrentDropPhotoProperty, value);
            }
        }

        public Series CurrentSeries
        {
            get { return (Series)GetValue(CurrentSeriesProperty); }

            set
            {
                SetValue(CurrentSeriesProperty, value);
            }
        }

        public string PixelsInMillimeter
        {
            get { return (string)GetValue(PixelsInMillimeterProperty); }
            set
            {
                SetValue(PixelsInMillimeterProperty, value);
            }
        }

        public string PixelsInMillimeterVertical
        {
            get { return (string)GetValue(PixelsInMillimeterVerticalProperty); }
            set
            {
                SetValue(PixelsInMillimeterVerticalProperty, value);
            }
        }

        public string PixelsInMillimeterHorizontal
        {
            get { return (string)GetValue(PixelsInMillimeterHorizontalProperty); }
            set
            {
                SetValue(PixelsInMillimeterHorizontalProperty, value);
            }
        }

        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(ImageSourceProperty); }

            set
            {
                SetValue(ImageSourceProperty, value);
            }
        }

        public PixelDrawer()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool _drawingHorizontalLine { get; set; }

        public bool _drawingVerticalLine { get; set; }

        public bool TwoLineMode { get; set; }

        private Line _selectedLine;

        private List<Line> _horizontalLines = new List<Line>();

        private List<Line> _verticalLines = new List<Line>();

        [DllImport("gdi32.dll")]
        private static extern bool LineDDA(int nXStart, int nYStart, int nXEnd, int nYEnd, LineDdaProc lpLineFunc, IntPtr lpData);

        private delegate void LineDdaProc(int x, int y, IntPtr lpData);

        private void canDrawing_MouseMove_NotDown(object sender, MouseEventArgs e)
        {
            Cursor newCursor;

            if (_drawingHorizontalLine)
            {
                newCursor = ((TextBlock)Application.Current.Resources["CursorRulerHorizontal"]).Cursor;
            }
            else if (_drawingVerticalLine)
            {
                newCursor = ((TextBlock)Application.Current.Resources["CursorRulerVertical"]).Cursor;
            }
            else
            {
                newCursor = ((TextBlock)Application.Current.Resources["CursorDrawing"]).Cursor;
            }

            if (CanDrawing.Cursor != newCursor)
                CanDrawing.Cursor = newCursor;
        }

        private void canDrawing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            CanDrawing.MouseMove -= canDrawing_MouseMove_NotDown;
            CanDrawing.MouseMove += canDrawing_MouseMove_Drawing;
            CanDrawing.MouseUp += canDrawing_MouseUp_Drawing;
            CanDrawing.MouseLeave += ReferenceImage_NewSegmentDrawingMouseLeave;
            Application.Current.Deactivated += ReferenceImage_NewSegmentDrawingLostFocus;

            _selectedLine = new Line
            {
                Stroke = Brushes.Yellow,
                X1 = e.GetPosition(Image).X,
                Y1 = e.GetPosition(Image).Y,
                X2 = e.GetPosition(Image).X,
                Y2 = e.GetPosition(Image).Y
            };

            CanDrawing.Children.Add(_selectedLine);
        }

        #region Drawing

        private void canDrawing_MouseMove_Drawing(object sender, MouseEventArgs e)
        {
            _selectedLine.X2 = e.GetPosition(Image).X;
            _selectedLine.Y2 = e.GetPosition(Image).Y;
        }

        private void canDrawing_MouseUp_Drawing(object sender, MouseEventArgs e)
        {
            CanDrawing.MouseMove -= canDrawing_MouseMove_Drawing;
            CanDrawing.MouseMove += canDrawing_MouseMove_NotDown;
            CanDrawing.MouseUp -= canDrawing_MouseUp_Drawing;
            CanDrawing.MouseLeave -= ReferenceImage_NewSegmentDrawingMouseLeave;
            Application.Current.Deactivated -= ReferenceImage_NewSegmentDrawingLostFocus;

            if ((_selectedLine.X1 == _selectedLine.X2) && (_selectedLine.Y1 == _selectedLine.Y2) || _selectedLine == null)
                CanDrawing.Children.Remove(_selectedLine);
            else
            {
                var point11 = new System.Drawing.Point(Convert.ToInt32(_selectedLine.X1), Convert.ToInt32(_selectedLine.Y1));
                var point22 = new System.Drawing.Point(Convert.ToInt32(_selectedLine.X2), Convert.ToInt32(_selectedLine.Y2));

                if (TwoLineMode)
                {
                    if (Math.Abs(_selectedLine.X1 - _selectedLine.X2) >= Math.Abs(_selectedLine.Y1 - _selectedLine.Y2) && !_drawingVerticalLine || _drawingHorizontalLine)
                    {
                        _selectedLine.Stroke = Brushes.DeepPink;

                        CanDrawing.Children.Remove(CurrentDropPhoto.HorizontalLine);

                        if (_horizontalLines.Count > 0)
                        {
                            CanDrawing.Children.Remove(_horizontalLines[_horizontalLines.Count - 1]);
                        }

                        _horizontalLines.Add(_selectedLine);
                        CurrentDropPhoto.HorizontalLine = _selectedLine;
                        CurrentDropPhoto.SimpleHorizontalLine = new SimpleLine
                        {
                            X1 = _selectedLine.X1,
                            X2 = _selectedLine.X2,
                            Y1 = _selectedLine.Y1,
                            Y2 = _selectedLine.Y2
                        };

                        PixelsInMillimeterHorizontal = GetPointsOnLine(point11, point22).Count.ToString();
                    }
                    else if (Math.Abs(_selectedLine.X1 - _selectedLine.X2) < Math.Abs(_selectedLine.Y1 - _selectedLine.Y2) && !_drawingHorizontalLine || _drawingVerticalLine)
                    {
                        _selectedLine.Stroke = Brushes.Green;

                        CanDrawing.Children.Remove(CurrentDropPhoto.VerticalLine);

                        if (_verticalLines.Count > 0)
                        {
                            CanDrawing.Children.Remove(_verticalLines[_verticalLines.Count - 1]);
                        }

                        _verticalLines.Add(_selectedLine);
                        CurrentDropPhoto.VerticalLine = _selectedLine;
                        CurrentDropPhoto.SimpleVerticalLine = new SimpleLine
                        {
                            X1 = _selectedLine.X1,
                            X2 = _selectedLine.X2,
                            Y1 = _selectedLine.Y1,
                            Y2 = _selectedLine.Y2
                        };

                        PixelsInMillimeterVertical = GetPointsOnLine(point11, point22).Count.ToString();
                    }
                }
                else
                {
                    _selectedLine.Stroke = Brushes.DeepPink;

                    CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                    CurrentSeries.ReferencePhotoForSeries.Line = _selectedLine;
                    CurrentSeries.ReferencePhotoForSeries.SimpleLine = new SimpleLine
                    {
                        X1 = _selectedLine.X1,
                        X2 = _selectedLine.X2,
                        Y1 = _selectedLine.Y1,
                        Y2 = _selectedLine.Y2
                    };

                    CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter = GetPointsOnLine(point11, point22).Count;
                }
            }
        }

        #endregion Drawing

        #region Control Leave Handlers

        private void ReferenceImage_NewSegmentDrawingMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                canDrawing_MouseUp_Drawing(sender, e);
            }
        }

        private void ReferenceImage_NewSegmentDrawingLostFocus(object sender, EventArgs e)
        {
            if (Mouse.RightButton == MouseButtonState.Pressed)
            {
                canDrawing_MouseUp_Drawing(sender, new MouseEventArgs(Mouse.PrimaryDevice, 0));
            }
        }

        #endregion

        public static List<Point> GetPointsOnLine(System.Drawing.Point point1, System.Drawing.Point point2)
        {
            var points = new List<Point>();
            var handle = GCHandle.Alloc(points);

            try
            {
                LineDDA(point1.X, point1.Y, point2.X, point2.Y, GetPointsOnLineCallback, GCHandle.ToIntPtr(handle));
            }
            finally
            {
                handle.Free();
            }

            return points;
        }

        private static void GetPointsOnLineCallback(int x, int y, IntPtr lpData)
        {
            var handle = GCHandle.FromIntPtr(lpData);
            var points = (List<Point>)handle.Target;
            points.Add(new Point(x, y));
        }
    }
}
