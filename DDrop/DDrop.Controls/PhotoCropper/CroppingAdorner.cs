﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using DDrop.Controls.PhotoCropper.Managers;
using DDrop.Controls.PhotoCropper.Utils;

namespace DDrop.Controls.PhotoCropper
{
    /// <summary>
    ///     Class that response for cropping images
    /// </summary>
    public class CroppingAdorner : Adorner
    {
        private readonly Canvas _canvasOverlay;
        private readonly DisplayTextManager _displayTextManager;
        private readonly Canvas _originalCanvas;
        private readonly OverlayManager _overlayManager;

        private readonly RectangleManager _rectangleManager;
        private readonly bool _squareMode;
        private readonly ThumbManager _thumbManager;
        private readonly VisualCollection _visualCollection;
        private bool _isMouseLeftButtonDown;

        public CroppingAdorner(UIElement adornedElement, bool squareMode) : base(adornedElement)
        {
            _squareMode = squareMode;
            _visualCollection = new VisualCollection(this);
            _originalCanvas = (Canvas) adornedElement;
            _canvasOverlay = new Canvas();
            _rectangleManager = new RectangleManager(_canvasOverlay, squareMode);
            _overlayManager = new OverlayManager(_canvasOverlay, _rectangleManager);
            _thumbManager = new ThumbManager(_canvasOverlay, _rectangleManager, squareMode);
            _displayTextManager = new DisplayTextManager(_canvasOverlay, _rectangleManager);
            _visualCollection.Add(_canvasOverlay);

            MouseLeftButtonDown += MouseLeftButtonDownEventHandler;
            MouseMove += MouseMoveEventHandler;
            MouseLeftButtonUp += MouseLeftButtonUpEventHandler;

            //add overlay
            Loaded += (sender, args) => { _overlayManager.UpdateOverlay(); };

            //if rectangle size chanhed, re-draw overlay
            _rectangleManager.RectangleSizeChanged += (sender, args) =>
            {
                _overlayManager.UpdateOverlay();
                _displayTextManager.UpdateSizeText();
            };
            _rectangleManager.OnRectangleDoubleClickEvent += (sender, args) =>
            {
                if (OnRectangleDoubleClickEvent != null)
                    OnRectangleDoubleClickEvent(sender, new DoubleClickEventArgs
                    {
                        BitmapFrame = GetCroppedBitmapFrame()
                    });
            };

            UpdateControls();
        }

        // Override the VisualChildrenCount properties to interface with 
        // the adorner's visual collection.
        protected override int VisualChildrenCount => _visualCollection.Count;

        public event EventHandler<DoubleClickEventArgs> OnRectangleDoubleClickEvent;

        /// <summary>
        ///     Get cropping areas as BitmapFrame
        /// </summary>
        /// <returns></returns>
        public BitmapFrame GetCroppedBitmapFrame()
        {
            var parent = VisualTreeHelper.GetParent(_originalCanvas) as UIElement;
            // 1) get current dpi
            var pSource = PresentationSource.FromVisual(Application.Current.MainWindow);
            var m = pSource.CompositionTarget.TransformToDevice;
            var dpiX = m.M11 * 96;
            var dpiY = m.M22 * 96;

            // 2) create RenderTargetBitmap
            var elementBitmap =
                new RenderTargetBitmap((int) _originalCanvas.ActualWidth, (int) _originalCanvas.ActualHeight, dpiX,
                    dpiY,
                    PixelFormats.Pbgra32);

            elementBitmap = new RenderTargetBitmap((int) _originalCanvas.RenderSize.Width,
                (int) _originalCanvas.RenderSize.Height, dpiX, dpiY, PixelFormats.Default);

            //Important
            _originalCanvas.Measure(_originalCanvas.RenderSize);
            _originalCanvas.Arrange(new Rect(_originalCanvas.RenderSize));

            // 3) draw element
            elementBitmap.Render(_originalCanvas);

            if (parent != null)
            {
                //Important
                parent.Measure(_originalCanvas.RenderSize);
                parent.Arrange(new Rect(_originalCanvas.RenderSize));
            }

            var crop = new CroppedBitmap(elementBitmap,
                new Int32Rect((int) _rectangleManager.TopLeft.X, (int) _rectangleManager.TopLeft.Y,
                    (int) _rectangleManager.RectangleWidth, (int) _rectangleManager.RectangleHeight));
            return BitmapFrame.Create(crop);
        }

        /// <summary>
        ///     Mouse left button down event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MouseLeftButtonDownEventHandler(object sender, MouseButtonEventArgs e)
        {
            _rectangleManager.MouseLeftButtonDownEventHandler(e);
            _overlayManager.UpdateOverlay();
            if (_rectangleManager.RectangleWidth == 0 && _rectangleManager.RectangleHeight == 0)
            {
                _thumbManager.ShowThumbs(false);
                _displayTextManager.ShowText(false);
            }

            _isMouseLeftButtonDown = true;
        }

        private void MouseMoveEventHandler(object sender, MouseEventArgs e)
        {
            if (_isMouseLeftButtonDown)
            {
                _rectangleManager.MouseMoveEventHandler(e);
                _overlayManager.UpdateOverlay();
                UpdateControls();
            }
        }

        private void UpdateControls()
        {
            _thumbManager.ShowThumbs(true);
            _displayTextManager.ShowText(true);
            _displayTextManager.UpdateSizeText();
            _thumbManager.UpdateThumbsPosition();
        }

        private void MouseLeftButtonUpEventHandler(object sender, MouseButtonEventArgs e)
        {
            _rectangleManager.MouseLeftButtonUpEventHandler();
            ReleaseMouseCapture();
            _isMouseLeftButtonDown = false;
        }

        // Override the GetVisualChild properties to interface with 
        // the adorner's visual collection.
        protected override Visual GetVisualChild(int index)
        {
            return _visualCollection[index];
        }

        // Positions child elements and determines a size
        protected override Size ArrangeOverride(Size size)
        {
            var finalSize = base.ArrangeOverride(size);
            _canvasOverlay.Arrange(new Rect(0, 0, AdornedElement.RenderSize.Width, AdornedElement.RenderSize.Height));
            return finalSize;
        }
    }
}