using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using DDrop.BE.Models;
using DDrop.Utility.Calculation;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Drawing.Point;

namespace DDrop.Controls.PixelDrawer
{
    /// <summary>
    ///     Interaction logic for PixelDrawer.xaml
    /// </summary>
    public partial class PixelDrawer
    {
        public static readonly DependencyProperty PixelsInMillimeterProperty =
            DependencyProperty.Register("PixelsInMillimeter", typeof(string), typeof(PixelDrawer));

        public static readonly DependencyProperty PixelsInMillimeterVerticalProperty =
            DependencyProperty.Register("PixelsInMillimeterVertical", typeof(string), typeof(PixelDrawer));

        public static readonly DependencyProperty PixelsInMillimeterHorizontalProperty =
            DependencyProperty.Register("PixelsInMillimeterHorizontal", typeof(string), typeof(PixelDrawer));

        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(PixelDrawer));

        public static readonly DependencyProperty CurrentSeriesProperty =
            DependencyProperty.Register("CurrentSeries", typeof(Series), typeof(PixelDrawer));

        public static readonly DependencyProperty CurrentDropPhotoProperty =
            DependencyProperty.Register("CurrentDropPhoto", typeof(DropPhoto), typeof(PixelDrawer));

        public static readonly DependencyProperty DrawningIsEnabledProperty =
            DependencyProperty.Register("DrawningIsEnabled", typeof(bool), typeof(PixelDrawer));

        private readonly List<Line> _horizontalLines = new List<Line>();

        private Line _selectedLine;

        private readonly List<Line> _verticalLines = new List<Line>();

        public PixelDrawer()
        {
            InitializeComponent();
            DataContext = this;
        }

        public bool DrawningIsEnabled
        {
            get => (bool) GetValue(DrawningIsEnabledProperty);
            set => SetValue(DrawningIsEnabledProperty, value);
        }

        public DropPhoto CurrentDropPhoto
        {
            get => (DropPhoto) GetValue(CurrentDropPhotoProperty);
            set => SetValue(CurrentDropPhotoProperty, value);
        }

        public Series CurrentSeries
        {
            get => (Series) GetValue(CurrentSeriesProperty);

            set => SetValue(CurrentSeriesProperty, value);
        }

        public string PixelsInMillimeter
        {
            get => (string) GetValue(PixelsInMillimeterProperty);
            set => SetValue(PixelsInMillimeterProperty, value);
        }

        public string PixelsInMillimeterVertical
        {
            get => (string) GetValue(PixelsInMillimeterVerticalProperty);
            set => SetValue(PixelsInMillimeterVerticalProperty, value);
        }

        public string PixelsInMillimeterHorizontal
        {
            get => (string) GetValue(PixelsInMillimeterHorizontalProperty);
            set => SetValue(PixelsInMillimeterHorizontalProperty, value);
        }

        public ImageSource ImageSource
        {
            get => (ImageSource) GetValue(ImageSourceProperty);

            set => SetValue(ImageSourceProperty, value);
        }

        public bool _drawingHorizontalLine { get; set; }

        public bool _drawingVerticalLine { get; set; }

        public bool TwoLineMode { get; set; }

        private void canDrawing_MouseMove_NotDown(object sender, MouseEventArgs e)
        {
            Cursor newCursor;

            if (_drawingHorizontalLine)
                newCursor = ((TextBlock) Application.Current.Resources["CursorRulerHorizontal"]).Cursor;
            else if (_drawingVerticalLine)
                newCursor = ((TextBlock) Application.Current.Resources["CursorRulerVertical"]).Cursor;
            else
                newCursor = ((TextBlock) Application.Current.Resources["CursorDrawing"]).Cursor;

            if (CanDrawing.Cursor != newCursor)
                CanDrawing.Cursor = newCursor;
        }

        private void canDrawing_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DrawningIsEnabled)
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

            if (_selectedLine.X1 == _selectedLine.X2 && _selectedLine.Y1 == _selectedLine.Y2 || _selectedLine == null)
            {
                CanDrawing.Children.Remove(_selectedLine);
            }
            else
            {
                var point11 = new Point(Convert.ToInt32(_selectedLine.X1), Convert.ToInt32(_selectedLine.Y1));
                var point22 = new Point(Convert.ToInt32(_selectedLine.X2), Convert.ToInt32(_selectedLine.Y2));

                if (TwoLineMode)
                {
                    if (Math.Abs(_selectedLine.X1 - _selectedLine.X2) >=
                        Math.Abs(_selectedLine.Y1 - _selectedLine.Y2) && !_drawingVerticalLine ||
                        _drawingHorizontalLine)
                    {
                        _selectedLine.Stroke = Brushes.DeepPink;

                        CanDrawing.Children.Remove(CurrentDropPhoto.HorizontalLine);

                        if (CurrentDropPhoto.Contour != null)
                        {
                            foreach (var line in CurrentDropPhoto.Contour.Lines) CanDrawing.Children.Remove(line);

                            CurrentDropPhoto.Contour.SimpleLines.Clear();
                            CurrentDropPhoto.Contour.Lines.Clear();

                            CurrentDropPhoto.Contour = null;
                        }

                        if (_horizontalLines.Count > 0)
                            CanDrawing.Children.Remove(_horizontalLines[_horizontalLines.Count - 1]);

                        _horizontalLines.Add(_selectedLine);
                        CurrentDropPhoto.HorizontalLine = _selectedLine;
                        var horizontalLineForAdd = new SimpleLine
                        {
                            X1 = _selectedLine.X1,
                            X2 = _selectedLine.X2,
                            Y1 = _selectedLine.Y1,
                            Y2 = _selectedLine.Y2
                        };

                        if (CurrentDropPhoto.SimpleHorizontalLine == null)
                        {
                            horizontalLineForAdd.SimpleLineId = Guid.NewGuid();
                            CurrentDropPhoto.SimpleHorizontalLine = horizontalLineForAdd;
                            CurrentDropPhoto.SimpleHorizontalLineId =
                                CurrentDropPhoto.SimpleHorizontalLine.SimpleLineId;
                        }
                        else
                        {
                            horizontalLineForAdd.SimpleLineId = CurrentDropPhoto.SimpleHorizontalLine.SimpleLineId;
                            CurrentDropPhoto.SimpleHorizontalLine = horizontalLineForAdd;
                        }

                        PixelsInMillimeterHorizontal =
                            LineLengthHelper.GetPointsOnLine(point11, point22).Count.ToString();
                        CurrentDropPhoto.XDiameterInPixels = Convert.ToInt32(PixelsInMillimeterHorizontal);
                    }
                    else if (Math.Abs(_selectedLine.X1 - _selectedLine.X2) <
                             Math.Abs(_selectedLine.Y1 - _selectedLine.Y2) && !_drawingHorizontalLine ||
                             _drawingVerticalLine)
                    {
                        _selectedLine.Stroke = Brushes.Green;

                        CanDrawing.Children.Remove(CurrentDropPhoto.VerticalLine);

                        if (CurrentDropPhoto.Contour != null)
                        {
                            foreach (var line in CurrentDropPhoto.Contour.Lines) CanDrawing.Children.Remove(line);

                            CurrentDropPhoto.Contour.SimpleLines.Clear();
                            CurrentDropPhoto.Contour.Lines.Clear();

                            CurrentDropPhoto.Contour = null;
                        }

                        if (_verticalLines.Count > 0)
                            CanDrawing.Children.Remove(_verticalLines[_verticalLines.Count - 1]);

                        _verticalLines.Add(_selectedLine);
                        CurrentDropPhoto.VerticalLine = _selectedLine;
                        var verticalLineForAdd = new SimpleLine
                        {
                            X1 = _selectedLine.X1,
                            X2 = _selectedLine.X2,
                            Y1 = _selectedLine.Y1,
                            Y2 = _selectedLine.Y2
                        };

                        if (CurrentDropPhoto.SimpleVerticalLine == null)
                        {
                            verticalLineForAdd.SimpleLineId = Guid.NewGuid();
                            CurrentDropPhoto.SimpleVerticalLine = verticalLineForAdd;
                            CurrentDropPhoto.SimpleVerticalLineId = CurrentDropPhoto.SimpleVerticalLine.SimpleLineId;
                        }
                        else
                        {
                            verticalLineForAdd.SimpleLineId = CurrentDropPhoto.SimpleVerticalLine.SimpleLineId;
                            CurrentDropPhoto.SimpleVerticalLine = verticalLineForAdd;
                        }

                        PixelsInMillimeterVertical =
                            LineLengthHelper.GetPointsOnLine(point11, point22).Count.ToString();
                        CurrentDropPhoto.YDiameterInPixels = Convert.ToInt32(PixelsInMillimeterVertical);
                    }
                }
                else
                {
                    _selectedLine.Stroke = Brushes.DeepPink;

                    CanDrawing.Children.Remove(CurrentSeries.ReferencePhotoForSeries.Line);

                    CurrentSeries.ReferencePhotoForSeries.Line = _selectedLine;
                    var simpleReferenceLineForAdd = new SimpleLine
                    {
                        X1 = _selectedLine.X1,
                        X2 = _selectedLine.X2,
                        Y1 = _selectedLine.Y1,
                        Y2 = _selectedLine.Y2
                    };

                    if (CurrentSeries.ReferencePhotoForSeries.SimpleLine == null)
                    {
                        simpleReferenceLineForAdd.SimpleLineId = Guid.NewGuid();
                        CurrentSeries.ReferencePhotoForSeries.SimpleLine = simpleReferenceLineForAdd;
                        CurrentSeries.ReferencePhotoForSeries.SimpleReferencePhotoLineId =
                            CurrentSeries.ReferencePhotoForSeries.SimpleLine.SimpleLineId;
                    }
                    else
                    {
                        simpleReferenceLineForAdd.SimpleLineId =
                            CurrentSeries.ReferencePhotoForSeries.SimpleLine.SimpleLineId;
                        CurrentSeries.ReferencePhotoForSeries.SimpleLine = simpleReferenceLineForAdd;
                    }

                    CurrentSeries.ReferencePhotoForSeries.PixelsInMillimeter =
                        LineLengthHelper.GetPointsOnLine(point11, point22).Count;
                }
            }
        }

        #endregion Drawing

        #region Control Leave Handlers

        private void ReferenceImage_NewSegmentDrawingMouseLeave(object sender, MouseEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed) canDrawing_MouseUp_Drawing(sender, e);
        }

        private void ReferenceImage_NewSegmentDrawingLostFocus(object sender, EventArgs e)
        {
            if (Mouse.RightButton == MouseButtonState.Pressed)
                canDrawing_MouseUp_Drawing(sender, new MouseEventArgs(Mouse.PrimaryDevice, 0));
        }

        #endregion
    }
}