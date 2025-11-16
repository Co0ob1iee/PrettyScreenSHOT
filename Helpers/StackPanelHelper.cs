using System.Windows;
using System.Windows.Controls;

namespace PrettyScreenSHOT.Helpers
{
    /// <summary>
    /// Helper class that adds Spacing support to StackPanel (WPF compatibility)
    /// </summary>
    public static class StackPanelHelper
    {
        /// <summary>
        /// Attached property for adding spacing between StackPanel children
        /// </summary>
        public static readonly DependencyProperty SpacingProperty =
            DependencyProperty.RegisterAttached(
                "Spacing",
                typeof(double),
                typeof(StackPanelHelper),
                new PropertyMetadata(0.0, OnSpacingChanged));

        public static double GetSpacing(DependencyObject obj)
        {
            return (double)obj.GetValue(SpacingProperty);
        }

        public static void SetSpacing(DependencyObject obj, double value)
        {
            obj.SetValue(SpacingProperty, value);
        }

        private static void OnSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is StackPanel stackPanel)
            {
                stackPanel.Loaded -= StackPanel_Loaded;
                stackPanel.Loaded += StackPanel_Loaded;

                if (stackPanel.IsLoaded)
                {
                    ApplySpacing(stackPanel);
                }
            }
        }

        private static void StackPanel_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is StackPanel stackPanel)
            {
                ApplySpacing(stackPanel);
            }
        }

        private static void ApplySpacing(StackPanel stackPanel)
        {
            var spacing = GetSpacing(stackPanel);
            var isHorizontal = stackPanel.Orientation == System.Windows.Controls.Orientation.Horizontal;

            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                if (stackPanel.Children[i] is FrameworkElement element)
                {
                    if (i == 0)
                    {
                        // First element - no spacing
                        element.Margin = new Thickness(0);
                    }
                    else
                    {
                        // Add spacing to the left (horizontal) or top (vertical)
                        if (isHorizontal)
                        {
                            element.Margin = new Thickness(spacing, 0, 0, 0);
                        }
                        else
                        {
                            element.Margin = new Thickness(0, spacing, 0, 0);
                        }
                    }
                }
            }
        }
    }
}
