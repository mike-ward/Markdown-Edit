using System.Windows;
using System.Windows.Media;

namespace MarkdownEdit.Controls
{
    public partial class FontComboBox
    {
        public static readonly DependencyProperty SelectedFontFamilyProperty = DependencyProperty.Register(
            "SelectedFontFamily", typeof(FontFamily), typeof(FontComboBox), new PropertyMetadata(default(FontFamily)));

        public static readonly DependencyProperty SelectedFontSizeProperty = DependencyProperty.Register(
            "SelectedFontSize", typeof(double), typeof(FontComboBox), new PropertyMetadata(default(double)));

        public FontComboBox() { InitializeComponent(); }

        public FontFamily SelectedFontFamily
        {
            get { return (FontFamily)GetValue(SelectedFontFamilyProperty); }
            set { SetValue(SelectedFontFamilyProperty, value); }
        }

        public double SelectedFontSize
        {
            get { return (double)GetValue(SelectedFontSizeProperty); }
            set { SetValue(SelectedFontSizeProperty, value); }
        }
    }
}