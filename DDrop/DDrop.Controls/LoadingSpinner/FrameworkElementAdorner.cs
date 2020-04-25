using System.Collections;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;

//
// This code based on code available here:
//
//  http://www.codeproject.com/KB/WPF/WPFJoshSmith.aspx
//
namespace DDrop.Controls.LoadingSpinner
{
    //
    // This class is an adorner that allows a FrameworkElement derived class to adorn another FrameworkElement.
    //
    public class FrameworkElementAdorner : Adorner
    {
        //
        // The framework element that is the adorner. 
        //
        private readonly FrameworkElement child;

        //
        // Placement of the child.
        //
        private readonly AdornerPlacement horizontalAdornerPlacement = AdornerPlacement.Inside;

        //
        // Offset of the child.
        //
        private readonly double offsetX;
        private readonly double offsetY;

        //
        // Position of the child (when not set to NaN).
        //
        private readonly AdornerPlacement verticalAdornerPlacement = AdornerPlacement.Inside;

        public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement)
            : base(adornedElement)
        {
            child = adornerChildElement;

            AddLogicalChild(adornerChildElement);
            AddVisualChild(adornerChildElement);
        }

        public FrameworkElementAdorner(FrameworkElement adornerChildElement, FrameworkElement adornedElement,
            AdornerPlacement horizontalAdornerPlacement, AdornerPlacement verticalAdornerPlacement,
            double offsetX, double offsetY)
            : base(adornedElement)
        {
            child = adornerChildElement;
            this.horizontalAdornerPlacement = horizontalAdornerPlacement;
            this.verticalAdornerPlacement = verticalAdornerPlacement;
            this.offsetX = offsetX;
            this.offsetY = offsetY;

            adornedElement.SizeChanged += adornedElement_SizeChanged;

            AddLogicalChild(adornerChildElement);
            AddVisualChild(adornerChildElement);
        }

        //
        // Position of the child (when not set to NaN).
        //
        public double PositionX { get; set; } = double.NaN;

        public double PositionY { get; set; } = double.NaN;

        protected override int VisualChildrenCount => 1;

        protected override IEnumerator LogicalChildren
        {
            get
            {
                var list = new ArrayList();
                list.Add(child);
                return list.GetEnumerator();
            }
        }

        /// <summary>
        ///     Override AdornedElement from base class for less type-checking.
        /// </summary>
        public new FrameworkElement AdornedElement => (FrameworkElement) base.AdornedElement;

        /// <summary>
        ///     Event raised when the adorned control's size has changed.
        /// </summary>
        private void adornedElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateMeasure();
        }

        protected override Size MeasureOverride(Size constraint)
        {
            child.Measure(constraint);
            return child.DesiredSize;
        }

        /// <summary>
        ///     Determine the X coordinate of the child.
        /// </summary>
        private double DetermineX()
        {
            switch (child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                {
                    if (horizontalAdornerPlacement == AdornerPlacement.Outside)
                        return -child.DesiredSize.Width + offsetX;
                    return offsetX;
                }

                case HorizontalAlignment.Right:
                {
                    if (horizontalAdornerPlacement == AdornerPlacement.Outside)
                    {
                        var adornedWidth = AdornedElement.ActualWidth;
                        return adornedWidth + offsetX;
                    }
                    else
                    {
                        var adornerWidth = child.DesiredSize.Width;
                        var adornedWidth = AdornedElement.ActualWidth;
                        var x = adornedWidth - adornerWidth;
                        return x + offsetX;
                    }
                }

                case HorizontalAlignment.Center:
                {
                    var adornerWidth = child.DesiredSize.Width;
                    var adornedWidth = AdornedElement.ActualWidth;
                    var x = adornedWidth / 2 - adornerWidth / 2;
                    return x + offsetX;
                }

                case HorizontalAlignment.Stretch:
                {
                    return 0.0;
                }
            }

            return 0.0;
        }

        /// <summary>
        ///     Determine the Y coordinate of the child.
        /// </summary>
        private double DetermineY()
        {
            switch (child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                {
                    if (verticalAdornerPlacement == AdornerPlacement.Outside)
                        return -child.DesiredSize.Height + offsetY;
                    return offsetY;
                }

                case VerticalAlignment.Bottom:
                {
                    if (verticalAdornerPlacement == AdornerPlacement.Outside)
                    {
                        var adornedHeight = AdornedElement.ActualHeight;
                        return adornedHeight + offsetY;
                    }
                    else
                    {
                        var adornerHeight = child.DesiredSize.Height;
                        var adornedHeight = AdornedElement.ActualHeight;
                        var x = adornedHeight - adornerHeight;
                        return x + offsetY;
                    }
                }

                case VerticalAlignment.Center:
                {
                    var adornerHeight = child.DesiredSize.Height;
                    var adornedHeight = AdornedElement.ActualHeight;
                    var x = adornedHeight / 2 - adornerHeight / 2;
                    return x + offsetY;
                }

                case VerticalAlignment.Stretch:
                {
                    return 0.0;
                }
            }

            return 0.0;
        }

        /// <summary>
        ///     Determine the width of the child.
        /// </summary>
        private double DetermineWidth()
        {
            if (!double.IsNaN(PositionX)) return child.DesiredSize.Width;

            switch (child.HorizontalAlignment)
            {
                case HorizontalAlignment.Left:
                {
                    return child.DesiredSize.Width;
                }

                case HorizontalAlignment.Right:
                {
                    return child.DesiredSize.Width;
                }

                case HorizontalAlignment.Center:
                {
                    return child.DesiredSize.Width;
                }

                case HorizontalAlignment.Stretch:
                {
                    return AdornedElement.ActualWidth;
                }
            }

            return 0.0;
        }

        /// <summary>
        ///     Determine the height of the child.
        /// </summary>
        private double DetermineHeight()
        {
            if (!double.IsNaN(PositionY)) return child.DesiredSize.Height;

            switch (child.VerticalAlignment)
            {
                case VerticalAlignment.Top:
                {
                    return child.DesiredSize.Height;
                }

                case VerticalAlignment.Bottom:
                {
                    return child.DesiredSize.Height;
                }

                case VerticalAlignment.Center:
                {
                    return child.DesiredSize.Height;
                }

                case VerticalAlignment.Stretch:
                {
                    return AdornedElement.ActualHeight;
                }
            }

            return 0.0;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var x = PositionX;
            if (double.IsNaN(x)) x = DetermineX();
            var y = PositionY;
            if (double.IsNaN(y)) y = DetermineY();
            var adornerWidth = DetermineWidth();
            var adornerHeight = DetermineHeight();
            child.Arrange(new Rect(x, y, adornerWidth, adornerHeight));
            return finalSize;
        }

        protected override Visual GetVisualChild(int index)
        {
            return child;
        }

        /// <summary>
        ///     Disconnect the child element from the visual tree so that it may be reused later.
        /// </summary>
        public void DisconnectChild()
        {
            RemoveLogicalChild(child);
            RemoveVisualChild(child);
        }
    }
}