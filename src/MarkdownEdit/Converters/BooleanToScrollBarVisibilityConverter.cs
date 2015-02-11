using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

// ReSharper disable once CheckNamespace
namespace MarkdownEdit.Conveters
{
    internal class BooleanToScrollBarVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool == false) return value;
            return (bool)value ? ScrollBarVisibility.Visible : ScrollBarVisibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}