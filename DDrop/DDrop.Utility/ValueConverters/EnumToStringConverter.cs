using System;
using System.Windows.Controls;
using System.Windows.Data;

namespace DDrop.Utility.ValueConverters
{
    public class EnumToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (SelectionMode)Enum.Parse(typeof(SelectionMode), value.ToString(), true);
        }
    }
}