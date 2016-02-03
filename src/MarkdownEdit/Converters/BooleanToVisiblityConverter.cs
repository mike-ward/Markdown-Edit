using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MarkdownEdit.Converters
{
    internal class BooleanToVisiblityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = value is bool && (bool)value;
            return (isTrue && parameter == null || !isTrue && parameter != null)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}