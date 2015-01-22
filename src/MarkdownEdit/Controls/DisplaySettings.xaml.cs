using System.Windows.Data;

namespace MarkdownEdit.Controls
{
    public partial class DisplaySettings
    {
        public DisplaySettings()
        {
            InitializeComponent();
            DataContext = App.UserSettings;

            var fontFamilyBinding = new Binding("EditorFontFamily") { Source = DataContext, Mode = BindingMode.TwoWay };
            FontCombo.SetBinding(FontComboBox.SelectedFontFamilyProperty, fontFamilyBinding);

            var fontSizeBinding = new Binding("EditorFontSize") { Source = DataContext, Mode = BindingMode.TwoWay };
            FontCombo.SetBinding(FontComboBox.SelectedFontSizeProperty, fontSizeBinding);
        }                                                           
    }
}