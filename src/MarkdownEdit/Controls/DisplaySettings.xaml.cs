using System.Linq;
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

            var modified = appSettings.GetType().GetProperties()
                .Select(property => new {appProperty = property, clonedProperty = _clonedSettings?.GetType().GetProperty(property.Name)})
                .Where(pair => pair.appProperty != null && pair.appProperty.CanWrite)
                .Where(pair => pair.appProperty.GetValue(appSettings).Equals(pair.clonedProperty.GetValue(_clonedSettings)) == false)
                .Select(pair => pair.appProperty)
                .Any();

            if (modified)
            {
                appSettings.Save();
            }
        }
    }
}