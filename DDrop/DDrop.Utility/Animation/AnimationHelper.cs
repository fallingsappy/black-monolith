using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using DDrop.Utility.Extensions;

namespace DDrop.Utility.Animation
{
    public class AnimationHelper
    {
        public static GridLengthAnimation animation;

        /// <summary>
        ///     Animate expand/collapse of a grid column.
        /// </summary>
        /// <param name="gridColumn">The grid column to expand/collapse.</param>
        /// <param name="expandedWidth">The expanded width.</param>
        /// <param name="milliseconds">The milliseconds component of the duration.</param>
        /// <param name="collapsedWidth">The width when collapsed.</param>
        /// <param name="minWidth">The minimum width of the column.</param>
        /// <param name="seconds">The seconds component of the duration.</param>
        /// <param name="expand">If true, expand, otherwise collapse.</param>
        public static async Task AnimateGridColumnExpandCollapseAsync(ColumnDefinition gridColumn, bool expand,
            double expandedWidth, double collapsedWidth,
            double minWidth, int seconds, int milliseconds)
        {
            if (expand && gridColumn.ActualWidth >= expandedWidth)
                // It's as wide as it needs to be.
                return;

            if (!expand && gridColumn.ActualWidth == collapsedWidth)
                // It's already collapsed.
                return;

            var storyBoard = new Storyboard();

            animation = new GridLengthAnimation();
            animation.From = new GridLength(gridColumn.ActualWidth);
            animation.To = new GridLength(expand ? expandedWidth : collapsedWidth);
            animation.Duration = new TimeSpan(0, 0, 0, seconds, milliseconds);

            // Set delegate that will fire on completion.
            animation.Completed += delegate
            {
                // Set the animation to null on completion. This allows the grid to be resized manually
                gridColumn.BeginAnimation(ColumnDefinition.WidthProperty, null);

                // Set the final value manually.
                gridColumn.Width = new GridLength(expand ? expandedWidth : collapsedWidth);

                // Set the minimum width.
                gridColumn.MinWidth = minWidth;
            };

            storyBoard.Children.Add(animation);

            Storyboard.SetTarget(animation, gridColumn);
            Storyboard.SetTargetProperty(animation, new PropertyPath(ColumnDefinition.WidthProperty));
            storyBoard.Children.Add(animation);

            // Begin the animation.
            await storyBoard.BeginAsync();
        }

        /// <summary>
        ///     Animate expand/collapse of a grid row.
        /// </summary>
        /// <param name="gridRow">The grid row to expand/collapse.</param>
        /// <param name="expandedHeight">The expanded height.</param>
        /// <param name="collapsedHeight">The collapesed height.</param>
        /// <param name="minHeight">The minimum height.</param>
        /// <param name="milliseconds">The milliseconds component of the duration.</param>
        /// <param name="seconds">The seconds component of the duration.</param>
        /// <param name="expand">If true, expand, otherwise collapse.</param>
        public static async Task AnimateGridRowExpandCollapse(RowDefinition gridRow, bool expand, double expandedHeight,
            double collapsedHeight, double minHeight, int seconds, int milliseconds)
        {
            if (expand && gridRow.ActualHeight >= expandedHeight)
                // It's as high as it needs to be.
                return;

            if (!expand && gridRow.ActualHeight == collapsedHeight)
                // It's already collapsed.
                return;

            var storyBoard = new Storyboard();

            var animation = new GridLengthAnimation();
            animation.From = new GridLength(gridRow.ActualHeight);
            animation.To = new GridLength(expand ? expandedHeight : collapsedHeight);
            animation.Duration = new TimeSpan(0, 0, 0, seconds, milliseconds);

            // Set delegate that will fire on completioon.
            animation.Completed += delegate
            {
                // Set the animation to null on completion. This allows the grid to be resized manually
                gridRow.BeginAnimation(RowDefinition.HeightProperty, null);

                // Set the final height.
                gridRow.Height = new GridLength(expand ? expandedHeight : collapsedHeight);

                // Set the minimum height.
                gridRow.MinHeight = minHeight;
            };

            storyBoard.Children.Add(animation);

            Storyboard.SetTarget(animation, gridRow);
            Storyboard.SetTargetProperty(animation, new PropertyPath(RowDefinition.HeightProperty));
            storyBoard.Children.Add(animation);

            // Begin the animation.
            await storyBoard.BeginAsync();
        }
    }
}