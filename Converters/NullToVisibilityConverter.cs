using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PrettyScreenSHOT.Converters
{
    /// <summary>
    /// Converts null or empty values to Visibility
    /// </summary>
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isInverse = parameter?.ToString() == "Inverse";
            bool isNull = value == null || (value is string str && string.IsNullOrEmpty(str));

            if (isInverse)
                return isNull ? Visibility.Visible : Visibility.Collapsed;
            else
                return !isNull ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
