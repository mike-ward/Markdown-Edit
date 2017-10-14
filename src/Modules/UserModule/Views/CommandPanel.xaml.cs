using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using MahApps.Metro.IconPacks;
using UserModule.Commands;

namespace UserModule.Views
{
    public partial class CommandPanel
    {
        public CommandPanel()
        {
            InitializeComponent();
            AddButtons();
        }

        private void AddButtons()
        {
            Panel.Children.Add(ButtonMaker(ApplicationCommands.New, GetMaterialIcon(PackIconMaterialKind.FileOutline)));
            Panel.Children.Add(ButtonMaker(ApplicationCommands.Open, GetMaterialIcon(PackIconMaterialKind.OpenInApp)));
            Panel.Children.Add(ButtonMaker(ApplicationCommands.Save, GetMaterialIcon(PackIconMaterialKind.ContentSave)));
            Panel.Children.Add(ButtonMaker(ApplicationCommands.SaveAs, GetMaterialIcon(PackIconMaterialKind.FileDocument)));
            Panel.Children.Add(ButtonMaker(DisplaySettingsCommandHandler.Command, GetMaterialIcon(PackIconMaterialKind.Settings)));

            Panel.Children.Add(new Rectangle
            {
                Width = 1,
                Height = 16,
                Margin = new Thickness(10, 0, 15, 0),
                Stroke = new SolidColorBrush(Colors.White),
                Opacity = 0.25
            });

            Panel.Children.Add(ButtonMaker(new RoutedCommand(), GetMaterialIcon(PackIconMaterialKind.Spellcheck)));
        }

        private static Button ButtonMaker(ICommand command, object content)
        {
            return new Button
            {
                Command = command,
                Content = content
            };
        }

        private static PackIconMaterial GetMaterialIcon(PackIconMaterialKind kind)
        {
            return new PackIconMaterial
            {
                Kind = kind,
                Margin = new Thickness(-5, 0, 0, 0),
                Width = 16,
                Height = 16
            };
        }
    }
}
