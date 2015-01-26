using System.Windows.Data;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    public partial class DisplaySettings
    {
        private UserSettings _clonedSettings;

        public DisplaySettings()
        {
            InitializeComponent();
            DataContext = App.UserSettings;

            var fontFamilyBinding = new Binding("EditorFontFamily") {Source = DataContext, Mode = BindingMode.TwoWay};
            FontCombo.SetBinding(FontComboBox.SelectedFontFamilyProperty, fontFamilyBinding);

            var fontSizeBinding = new Binding("EditorFontSize") {Source = DataContext, Mode = BindingMode.TwoWay};
            FontCombo.SetBinding(FontComboBox.SelectedFontSizeProperty, fontSizeBinding);
        }

        public void SaveState()
        {
            _clonedSettings = ((UserSettings)DataContext).Clone() as UserSettings;
        }

        public void SaveIfModified()
        {
            var appSettings = (UserSettings)DataContext;
            if (appSettings != _clonedSettings)
            {
                appSettings.Save();
            }
        }
    }
}