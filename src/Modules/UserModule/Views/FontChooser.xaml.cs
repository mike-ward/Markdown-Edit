using System.Windows;
using System.Windows.Media;

namespace UserModule.Views
{
    public partial class FontChooser
    {
        public static readonly DependencyProperty SelectedFontFamilyProperty = DependencyProperty.Register(
            "SelectedFontFamily", typeof(FontFamily), typeof(FontChooser), new PropertyMetadata(default(FontFamily)));

        public static readonly DependencyProperty SelectedFontSizeProperty = DependencyProperty.Register(
            "SelectedFontSize", typeof(double), typeof(FontChooser), new PropertyMetadata(default(double)));

        public FontChooser()
        {
            InitializeComponent();
        }

        public FontFamily SelectedFontFamily
        {
            get => (FontFamily)GetValue(SelectedFontFamilyProperty);
            set => SetValue(SelectedFontFamilyProperty, value);
        }

        public double SelectedFontSize
        {
            // ReSharper disable once PossibleNullReferenceException
            get => (double)GetValue(SelectedFontSizeProperty);
            set => SetValue(SelectedFontSizeProperty, value);
        }
    }
}
