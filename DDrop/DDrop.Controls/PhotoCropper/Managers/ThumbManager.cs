using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DDrop.Controls.PhotoCropper.Thumbs;

namespace DDrop.Controls.PhotoCropper.Managers
{
    /// <summary>
    ///     у11
    ///     Class that represent for displaying/redraw thumbs
    /// </summary>
    internal class ThumbManager
    {
        private readonly ThumbCrop _bottomMiddle,
            _leftMiddle,
            _topMiddle,
            _rightMiddle,
            _topLeft,
            _topRight,
            _bottomLeft,
            _bottomRight;

        private readonly Canvas _canvas;
        private readonly RectangleManager _rectangleManager;
        private readonly bool _squareMode;
        private readonly double _thumbSize;

        public ThumbManager(Canvas canvas, RectangleManager rectangleManager, bool squareMode)
        {
            //  initizalize
            _canvas = canvas;
            _squareMode = squareMode;
            _rectangleManager = rectangleManager;
            _thumbSize = 10;

            //  create thumbs with factory
            _bottomMiddle = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.BottomMiddle, _canvas, _thumbSize);
            _leftMiddle = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.LeftMiddle, _canvas, _thumbSize);
            _topMiddle = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.TopMiddle, _canvas, _thumbSize);
            _rightMiddle = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.RightMiddle, _canvas, _thumbSize);
            _topLeft = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.TopLeft, _canvas, _thumbSize);
            _topRight = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.TopRight, _canvas, _thumbSize);
            _bottomLeft = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.BottomLeft, _canvas, _thumbSize);
            _bottomRight = ThumbFactory.CreateThumb(ThumbFactory.ThumbPosition.BottomRight, _canvas, _thumbSize);

            //  subsctibe to mouse events
            _bottomMiddle.DragDelta += BottomMiddleDragDeltaEventHandler;
            _bottomMiddle.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDownGenericHandler;
            _bottomMiddle.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUpGenericHandler;

            _leftMiddle.DragDelta += LeftMiddleDragDeltaEventHandler;
            _leftMiddle.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDownGenericHandler;
            _leftMiddle.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUpGenericHandler;

            _topMiddle.DragDelta += TopMiddleDragDeltaEventHandler;
            _topMiddle.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDownGenericHandler;
            _topMiddle.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUpGenericHandler;

            _rightMiddle.DragDelta += RightMiddleDragDeltaEventHandler;
            _rightMiddle.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDownGenericHandler;
            _rightMiddle.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUpGenericHandler;

            _topLeft.DragDelta += TopLeftDragDeltaEventHandler;
            _topLeft.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDownGenericHandler;
            _topLeft.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUpGenericHandler;

            _topRight.DragDelta += TopRightDragDeltaEventHandler;
            _topRight.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDownGenericHandler;
            _topRight.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUpGenericHandler;

            _bottomLeft.DragDelta += BottomLeftDragDeltaEventHandler;
            _bottomLeft.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDownGenericHandler;
            _bottomLeft.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUpGenericHandler;

            _bottomRight.DragDelta += BottomRightDragDeltaEventHandler;
            _bottomRight.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDownGenericHandler;
            _bottomRight.PreviewMouseLeftButtonUp += PreviewMouseLeftButtonUpGenericHandler;
        }

        private double BottomSideCalculation(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as ThumbCrop;
            var deltaVertical = e.VerticalChange;
            var currentPoint = Canvas.GetTop(thumb);
            var thumbResultTop = currentPoint + deltaVertical;
            if (thumbResultTop + _thumbSize / 2 > _canvas.ActualHeight)
                thumbResultTop = _canvas.ActualHeight - _thumbSize / 2;
            var resultHeight = thumbResultTop - _rectangleManager.TopLeft.Y + _thumbSize / 2;
            return resultHeight;
        }

        private double RightSideCalculation(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as ThumbCrop;

            var deltaHorizontal = e.HorizontalChange;
            var currLeft = Canvas.GetLeft(thumb);
            var resultThumbLeft = currLeft + deltaHorizontal;
            if (resultThumbLeft > _canvas.ActualWidth) resultThumbLeft = _canvas.ActualWidth;

            var resultWidth = resultThumbLeft - _rectangleManager.TopLeft.X;
            return resultWidth;
        }

        private Tuple<double, double> LeftSideCalculation(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as ThumbCrop;
            var deltaHorizontal = e.HorizontalChange;
            var currLeft = Canvas.GetLeft(thumb);
            var resulThumbtLeft = currLeft + deltaHorizontal;
            if (resulThumbtLeft < 0) resulThumbtLeft = -_thumbSize / 2;

            var offset = currLeft - resulThumbtLeft;
            var resultLeft = resulThumbtLeft + _thumbSize / 2;
            var resultWidth = _rectangleManager.RectangleWidth + offset;
            return Tuple.Create(resultLeft, resultWidth);
        }

        private Tuple<double, double> TopSideCalculation(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as ThumbCrop;
            var deltaVertical = e.VerticalChange;
            var currentPoint = Canvas.GetTop(thumb);
            var resultThumbTop = currentPoint + deltaVertical;
            if (resultThumbTop < 0) resultThumbTop = -_thumbSize / 2;
            var offset = currentPoint - resultThumbTop;
            var resultHeight = _rectangleManager.RectangleHeight + offset;
            var resultTop = resultThumbTop + _thumbSize / 2;
            return Tuple.Create(resultTop, resultHeight);
        }

        private Tuple<double, double> LeftSideCalculationSquare(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as ThumbCrop;
            var deltaVertical = e.HorizontalChange;
            var currLeft = Canvas.GetLeft(thumb);
            var resulThumbtLeft = currLeft + deltaVertical;

            if (resulThumbtLeft < 0) resulThumbtLeft = -_thumbSize / 2;

            var offset = currLeft - resulThumbtLeft;
            var resultLeft = resulThumbtLeft + _thumbSize / 2;
            var resultWidth = _rectangleManager.RectangleWidth + offset;
            return Tuple.Create(resultLeft, resultWidth);
        }

        private Tuple<double, double> TopSideCalculationSquare(object sender, DragDeltaEventArgs e)
        {
            var thumb = sender as ThumbCrop;
            var deltaHorizontal = e.HorizontalChange;
            var currentPoint = Canvas.GetTop(thumb);
            var getLeft = Canvas.GetLeft(thumb);
            var resultThumbTop = currentPoint;
            if (getLeft > 0)
                resultThumbTop = currentPoint + deltaHorizontal;

            if (resultThumbTop < 0) resultThumbTop = -_thumbSize / 2;
            var offset = currentPoint - resultThumbTop;
            var resultHeight = _rectangleManager.RectangleHeight + offset;
            var resultTop = resultThumbTop + _thumbSize / 2;
            return Tuple.Create(resultTop, resultHeight);
        }

        private void BottomRightDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            if (_squareMode)
            {
                var resultHeight = BottomSideCalculation(sender, e);
                UpdateRectangeSize(null, null, resultHeight, resultHeight);
            }
            else
            {
                var resultWidth = RightSideCalculation(sender, e);
                var resultHeight = BottomSideCalculation(sender, e);

                UpdateRectangeSize(null, null, resultHeight, resultWidth);
            }
        }

        private void BottomLeftDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            if (_squareMode)
            {
                var resultLeft = LeftSideCalculation(sender, e).Item1;
                var resultWidth = LeftSideCalculation(sender, e).Item2;
                UpdateRectangeSize(resultLeft, null, resultWidth, resultWidth);
            }
            else
            {
                var resultHeight = BottomSideCalculation(sender, e);
                var resultLeft = LeftSideCalculation(sender, e).Item1;
                var resultWidth = LeftSideCalculation(sender, e).Item2;

                UpdateRectangeSize(resultLeft, null, resultHeight, resultWidth);
            }
        }

        private void TopRightDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            if (_squareMode)
            {
                var resultTop = TopSideCalculation(sender, e).Item1;
                var resultHeight = TopSideCalculation(sender, e).Item2;
                UpdateRectangeSize(null, resultTop, resultHeight, resultHeight);
            }
            else
            {
                var resultTop = TopSideCalculation(sender, e).Item1;
                var resultHeight = TopSideCalculation(sender, e).Item2;
                var resultWidth = RightSideCalculation(sender, e);

                UpdateRectangeSize(null, resultTop, resultHeight, resultWidth);
            }
        }

        private void TopLeftDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            if (_squareMode)
            {
                var resultTop = TopSideCalculationSquare(sender, e).Item1;
                var resultLeft = LeftSideCalculation(sender, e).Item1;
                var resultWidth = LeftSideCalculation(sender, e).Item2;
                UpdateRectangeSize(resultLeft, resultTop, resultWidth, resultWidth);
            }
            else
            {
                var resultTop = TopSideCalculation(sender, e).Item1;
                var resultHeight = TopSideCalculation(sender, e).Item2;
                var resultLeft = LeftSideCalculation(sender, e).Item1;
                var resultWidth = LeftSideCalculation(sender, e).Item2;

                UpdateRectangeSize(resultLeft, resultTop, resultHeight, resultWidth);
            }
        }

        private void RightMiddleDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            var resultWidth = RightSideCalculation(sender, e);
            UpdateRectangeSize(null, null, null, resultWidth);
        }

        private void TopMiddleDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            var resultTop = TopSideCalculation(sender, e).Item1;
            var resultHeight = TopSideCalculation(sender, e).Item2;
            UpdateRectangeSize(null, resultTop, resultHeight, null);
        }

        private void LeftMiddleDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            var resultLeft = LeftSideCalculation(sender, e).Item1;
            var resultWidth = LeftSideCalculation(sender, e).Item2;
            UpdateRectangeSize(resultLeft, null, null, resultWidth);
        }

        private void BottomMiddleDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            var resultHeight = BottomSideCalculation(sender, e);
            UpdateRectangeSize(null, null, resultHeight, null);
        }

        /// <summary>
        ///     Update (redraw) thumbs positions
        /// </summary>
        public void UpdateThumbsPosition()
        {
            if (_rectangleManager.RectangleHeight > 0 && _rectangleManager.RectangleWidth > 0)
            {
                _bottomMiddle.SetPosition(_rectangleManager.TopLeft.X + _rectangleManager.RectangleWidth / 2,
                    _rectangleManager.TopLeft.Y + _rectangleManager.RectangleHeight);
                _leftMiddle.SetPosition(_rectangleManager.TopLeft.X,
                    _rectangleManager.TopLeft.Y + _rectangleManager.RectangleHeight / 2);
                _topMiddle.SetPosition(_rectangleManager.TopLeft.X + _rectangleManager.RectangleWidth / 2,
                    _rectangleManager.TopLeft.Y);
                _rightMiddle.SetPosition(_rectangleManager.TopLeft.X + _rectangleManager.RectangleWidth,
                    _rectangleManager.TopLeft.Y + _rectangleManager.RectangleHeight / 2);
                _topLeft.SetPosition(_rectangleManager.TopLeft.X, _rectangleManager.TopLeft.Y);
                _topRight.SetPosition(_rectangleManager.TopLeft.X + _rectangleManager.RectangleWidth,
                    _rectangleManager.TopLeft.Y);
                _bottomLeft.SetPosition(_rectangleManager.TopLeft.X,
                    _rectangleManager.TopLeft.Y + _rectangleManager.RectangleHeight);
                _bottomRight.SetPosition(_rectangleManager.TopLeft.X + _rectangleManager.RectangleWidth,
                    _rectangleManager.TopLeft.Y + _rectangleManager.RectangleHeight);
            }
        }

        /// <summary>
        ///     Manage thumbs visibility
        /// </summary>
        /// <param name="isVisble">Set current visibility</param>
        public void ShowThumbs(bool isVisble)
        {
            if (_squareMode)
            {
                if (isVisble && _rectangleManager.RectangleHeight > 0 && _rectangleManager.RectangleWidth > 0)
                {
                    _topLeft.Visibility = Visibility.Visible;
                    _topRight.Visibility = Visibility.Visible;
                    _bottomLeft.Visibility = Visibility.Visible;
                    _bottomRight.Visibility = Visibility.Visible;
                }
                else
                {
                    _topLeft.Visibility = Visibility.Hidden;
                    _topRight.Visibility = Visibility.Hidden;
                    _bottomLeft.Visibility = Visibility.Hidden;
                    _bottomRight.Visibility = Visibility.Hidden;
                }
            }
            else
            {
                if (isVisble && _rectangleManager.RectangleHeight > 0 && _rectangleManager.RectangleWidth > 0)
                {
                    _bottomMiddle.Visibility = Visibility.Visible;
                    _leftMiddle.Visibility = Visibility.Visible;
                    _topMiddle.Visibility = Visibility.Visible;
                    _rightMiddle.Visibility = Visibility.Visible;
                    _topLeft.Visibility = Visibility.Visible;
                    _topRight.Visibility = Visibility.Visible;
                    _bottomLeft.Visibility = Visibility.Visible;
                    _bottomRight.Visibility = Visibility.Visible;
                }
                else
                {
                    _bottomMiddle.Visibility = Visibility.Hidden;
                    _leftMiddle.Visibility = Visibility.Hidden;
                    _topMiddle.Visibility = Visibility.Hidden;
                    _rightMiddle.Visibility = Visibility.Hidden;
                    _topLeft.Visibility = Visibility.Hidden;
                    _topRight.Visibility = Visibility.Hidden;
                    _bottomLeft.Visibility = Visibility.Hidden;
                    _bottomRight.Visibility = Visibility.Hidden;
                }
            }
        }

        /// <summary>
        ///     Update cropping rectangle
        /// </summary>
        /// <param name="left">Left rectangle coordinate</param>
        /// <param name="top">Top rectangle coordinate</param>
        /// <param name="height">Height of rectangle</param>
        /// <param name="width">Width of rectangle</param>
        private void UpdateRectangeSize(double? left, double? top, double? height, double? width)
        {
            var resultLeft = _rectangleManager.TopLeft.X;
            var resultTop = _rectangleManager.TopLeft.Y;
            var resultHeight = _rectangleManager.RectangleHeight;
            var resultWidth = _rectangleManager.RectangleWidth;

            if (left != null)
                resultLeft = (double) left;
            if (top != null)
                resultTop = (double) top;
            if (height != null)
                resultHeight = (double) height;
            if (width != null)
                resultWidth = (double) width;

            _rectangleManager.UpdateRectangle(resultLeft, resultTop, resultWidth, resultHeight);
            UpdateThumbsPosition();
        }

        private void PreviewMouseLeftButtonDownGenericHandler(object sender, MouseButtonEventArgs e)
        {
            var thumb = sender as ThumbCrop;
            thumb.CaptureMouse();
        }

        private void PreviewMouseLeftButtonUpGenericHandler(object sender, MouseButtonEventArgs e)
        {
            var thumb = sender as ThumbCrop;
            thumb.ReleaseMouseCapture();
        }
    }
}