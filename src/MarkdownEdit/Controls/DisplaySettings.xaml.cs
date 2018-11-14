using System.Diagnostics;
using System.Windows;
using System.Windows.Data;
using System.Windows.Navigation;
using MarkdownEdit.Models;
using MarkdownEdit.SpellCheck;

namespace MarkdownEdit.Controls
{
    public partial class DisplaySettings
    {
        public static readonly DependencyProperty SpellCheckProviderProperty = DependencyProperty.Register(
            "SpellCheckProvider", typeof(ISpellCheckProvider), typeof(DisplaySettings),
            new PropertyMetadata(default(ISpellCheckProvider)));

        private UserSettings _clonedSettings;

        public DisplaySettings()
        {
            InitializeComponent();
            IsVisibleChanged += OnIsVisibleChanged;
        }

        public ISpellCheckProvider SpellCheckProvider
        {
            get => (ISpellCheckProvider)GetValue(SpellCheckProviderProperty);
            set => SetValue(SpellCheckProviderProperty, value);
        }

        private void OnIsVisibleChanged(object sender,
            DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            IsVisibleChanged -= OnIsVisibleChanged;
            // This should work from XAML, but doesn't
            var fontFamilyBinding = new Binding("EditorFontFamily") {Source = DataContext, Mode = BindingMode.TwoWay};
            FontCombo.SetBinding(FontComboBox.SelectedFontFamilyProperty, fontFamilyBinding);
            var fontSizeBinding = new Binding("EditorFontSize") {Source = DataContext, Mode = BindingMode.TwoWay};
            FontCombo.SetBinding(FontComboBox.SelectedFontSizeProperty, fontSizeBinding);
            var encodingBinding = new Binding("EditorEncoding") {Source = DataContext, Mode = BindingMode.TwoWay};
            EncodingComboBox.SetBinding(EncodingComboBox.SelectedEncodingProperty, encodingBinding);
            //MarkdownEngine.IsEnabled = string.IsNullOrWhiteSpace(App.UserSettings.CustomMarkdownConverter);
        }

        public void SaveState() { _clonedSettings = ((UserSettings)DataContext).Clone() as UserSettings; }

        public void SaveIfModified()
        {
            var appSettings = (UserSettings)DataContext;
            if (appSettings.Equals(_clonedSettings) == false)
            {
                appSettings.Save();
            }
        }

        private void HyperlinkOnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}