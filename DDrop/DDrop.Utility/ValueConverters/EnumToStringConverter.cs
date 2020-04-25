using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace DDrop.Utility.ValueConverters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (SelectionMode) Enum.Parse(typeof(SelectionMode), value.ToString(), true);
        }
    }
}