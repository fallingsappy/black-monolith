using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace DDrop.Utility.Animation
{
    public class GridLengthAnimation : AnimationTimeline
    {
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        public GridLength From
        {
            get => (GridLength) GetValue(FromProperty);
            set => SetValue(FromProperty, value);
        }

        public GridLength To
        {
            get => (GridLength) GetValue(ToProperty);
            set => SetValue(ToProperty, value);
        }

        public override Type TargetPropertyType => typeof(GridLength);

        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        public override object GetCurrentValue(object defaultOriginValue, object defaultDestinationValue,
            AnimationClock animationClock)
        {
            var fromValue = From.Value;
            var toValue = To.Value;

            if (fromValue > toValue)
                return new GridLength((1 - animationClock.CurrentProgress.Value) * (fromValue - toValue) + toValue,
                    To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            return new GridLength(animationClock.CurrentProgress.Value * (toValue - fromValue) + fromValue,
                To.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
        }
    }
}