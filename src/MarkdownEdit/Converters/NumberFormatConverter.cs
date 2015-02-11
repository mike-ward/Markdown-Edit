using System;
using System.Globalization;
using System.Windows.Data;

// ReSharper disable once CheckNamespace

namespace MarkdownEdit.Conveters
{
    internal class NumberFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is int) ? ((int)value).ToString("N0") : value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}