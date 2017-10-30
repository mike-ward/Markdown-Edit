using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using Infrastructure;
using MahApps.Metro.IconPacks;
using UserModule.Commands;

namespace UserModule.Views
{
    public partial class CommandPanel
    {
        private readonly ISettings _settings;

        public CommandPanel(ISettings settings)
        {
            _settings = settings;
            InitializeComponent();
            AddButtons();
        }

        private void AddButtons()
        {
            // todo: localize
            Panel.Children.Add(ButtonMaker(ApplicationCommands.New, GetMaterialIcon(PackIconMaterialKind.FileOutline), "New - Ctrl+N"));
            Panel.Children.Add(ButtonMaker(ApplicationCommands.Open, GetMaterialIcon(PackIconMaterialKind.OpenInApp), "Open - Ctrl+O"));
            Panel.Children.Add(ButtonMaker(ApplicationCommands.Save, GetMaterialIcon(PackIconMaterialKind.ContentSave), "Save, - Ctrl + S"));
            Panel.Children.Add(ButtonMaker(ApplicationCommands.SaveAs, GetMaterialIcon(PackIconMaterialKind.FileDocument), "Save As - Ctrl+Shift+S"));
            Panel.Children.Add(ButtonMaker(DisplaySettingsCommandHandler.Command, GetMaterialIcon(PackIconMaterialKind.Settings), "Settings - Ctrl+Comma"));

            Panel.Children.Add(new Rectangle
            {
                Width = 1,
                Height = 16,
                Margin = new Thickness(10, 0, 15, 0),
                Stroke = new SolidColorBrush(Colors.White),
                Opacity = 0.25
            });

            Panel.Children.Add(ButtonMaker(new RoutedCommand(), GetMaterialIcon(PackIconMaterialKind.Spellcheck), "Spell Check - Ctrl+F7"));
            Panel.Children.Add(ToggleButtonMaker(_settings, "WordWrap", GetMaterialIcon(PackIconMaterialKind.Wrap), "Word Wrap - Ctrl+W"));
            Panel.Children.Add(ButtonMaker(new RoutedCommand(), GetMaterialIcon(PackIconMaterialKind.BackupRestore), "Autosave - Alt+S"));
        }

        private static Button ButtonMaker(ICommand command, object content, string tooltip)
        {
            return new Button
            {
                Command = command,
                Content = content,
                ToolTip = tooltip
            };
        }

        private ToggleButton ToggleButtonMaker(object context, string prop, object content, string tooltip)
        {
            var toggle = new ToggleButton
            {
                Content = content,
                ToolTip = tooltip,
                DataContext = context
            };

            toggle.SetBinding(ToggleButton.IsCheckedProperty, prop);
            return toggle;
        }

        private static PackIconMaterial GetMaterialIcon(PackIconMaterialKind kind)
        {
            return new PackIconMaterial
            {
                Kind = kind,
                Margin = new Thickness(-5, 0, 0, 0),
                Width = 14,
                Height = 16
            };
        }
    }
}
