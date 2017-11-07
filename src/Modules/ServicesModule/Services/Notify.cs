using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infrastructure;
using MahApps.Metro.IconPacks;
using MahApps.Metro.SimpleChildWindow;

namespace ServicesModule.Services
{
    public class Notify : INotify
    {
        private readonly double _fontSize = 18;
        private readonly double _buttonWidth = 100;
        private readonly double _buttonColumnWidth = 110;
        private readonly Thickness _buttonMargin = new Thickness(0, 10, 0, 0);

        public async Task<bool> Alert(string alert)
        {
            var dialog = Dialog();
            var content = (Grid)dialog.Content;
            var icon = GetMaterialIcon(PackIconMaterialKind.AlertCircle);
            var message = MessageBlock(alert);
            var buttons = ButtonBlock();

            buttons.SetValue(Grid.RowProperty, 2);
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });

            var ok = new Button { Content = "OK", Width = _buttonWidth };
            ok.SetValue(Grid.RowProperty, 2);
            ok.SetValue(Grid.ColumnProperty, 1);
            ok.Click += (sd, ea) => dialog.Close();

            buttons.Children.Add(ok);

            content.Children.Add(icon);
            content.Children.Add(message);
            content.Children.Add(buttons);

            await ShowDialog(dialog);
            return true;
        }

        public async Task<MessageBoxResult> ConfirmYesNo(string question)
        {
            var result = MessageBoxResult.No;
            var dialog = Dialog();
            var content = (Grid)dialog.Content;
            var icon = GetMaterialIcon(PackIconMaterialKind.HelpCircle);
            var message = MessageBlock(question);
            var buttons = ButtonBlock();

            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });

            var yes = new Button { Content = "_Yes", Width = _buttonWidth };
            yes.SetValue(Grid.RowProperty, 2);
            yes.SetValue(Grid.ColumnProperty, 1);
            yes.Click += (sd, ea) =>
            {
                result = MessageBoxResult.Yes;
                dialog.Close();
            };
            var no = new Button { Content = "_No", Width = _buttonWidth };
            no.SetValue(Grid.RowProperty, 2);
            no.SetValue(Grid.ColumnProperty, 2);
            no.Click += (sd, ea) =>
            {
                result = MessageBoxResult.No;
                dialog.Close();
            };

            buttons.Children.Add(yes);
            buttons.Children.Add(no);

            content.Children.Add(icon);
            content.Children.Add(message);
            content.Children.Add(buttons);

            await ShowDialog(dialog);
            return result;
        }

        public async Task<MessageBoxResult> ConfirmYesNoCancel(string question)
        {
            var result = MessageBoxResult.Cancel;
            var dialog = Dialog();
            var content = (Grid)dialog.Content;

            var icon = GetMaterialIcon(PackIconMaterialKind.HelpCircle);
            var message = MessageBlock(question);
            var buttons = ButtonBlock();

            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });

            var yes = new Button { Content = "_Yes", Width = _buttonWidth };
            yes.SetValue(Grid.RowProperty, 2);
            yes.SetValue(Grid.ColumnProperty, 1);
            yes.Click += (sd, ea) =>
            {
                result = MessageBoxResult.Yes;
                dialog.Close();
            };
            var no = new Button { Content = "_No", Width = _buttonWidth };
            no.SetValue(Grid.RowProperty, 2);
            no.SetValue(Grid.ColumnProperty, 2);
            no.Click += (sd, ea) =>
            {
                result = MessageBoxResult.No;
                dialog.Close();
            };
            var cancel = new Button { Content = "_Cancel", Width = _buttonWidth };
            cancel.SetValue(Grid.ColumnProperty, 3);
            cancel.Click += (sd, ea) =>
            {
                result = MessageBoxResult.Cancel;
                dialog.Close();
            };

            buttons.Children.Add(yes);
            buttons.Children.Add(no);
            buttons.Children.Add(cancel);

            content.Children.Add(icon);
            content.Children.Add(message);
            content.Children.Add(buttons);

            await ShowDialog(dialog);
            return result;
        }

        private static ChildWindow Dialog()
        {
            var grid = new Grid { Margin = new Thickness(20), MinHeight = 100, Width = 550 };

            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(50)});
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(30)});
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var overlay = new SolidColorBrush
            {
                Opacity = 0.75,
                // ReSharper disable once PossibleNullReferenceException
                Color = (Color)Application.Current.MainWindow.FindResource("Gray1")
            };
            overlay.Freeze();

            var dialog = new ChildWindow
            {
                Content = grid,
                OverlayBrush = overlay,
                ShowTitleBar = false
                
            };

            return dialog;
        }

        private static async Task ShowDialog(ChildWindow dialog)
        {
            await Application.Current.MainWindow.ShowChildWindowAsync(dialog, ChildWindowManager.OverlayFillBehavior.FullWindow);
        }

        private static PackIconMaterial GetMaterialIcon(PackIconMaterialKind kind)
        {
            var icon = new PackIconMaterial
            {
                Kind = kind,
                Width = 50,
                Height = 50,
                Foreground = new SolidColorBrush(Colors.SteelBlue),
                VerticalAlignment = VerticalAlignment.Center
            };

            icon.SetValue(Grid.RowProperty, 0);
            icon.SetValue(Grid.ColumnProperty, 0);
            icon.SetValue(Grid.RowSpanProperty, 2);

            return icon;
        }

        private TextBlock MessageBlock(string text)
        {
            var message = new TextBlock { Text = text, FontSize = _fontSize };
            message.SetValue(Grid.RowProperty, 0);
            message.SetValue(Grid.ColumnProperty, 2);
            return message;
        }

        private Grid ButtonBlock()
        {
            var buttons = new Grid { Margin = _buttonMargin };
            buttons.SetValue(Grid.RowProperty, 2);
            buttons.SetValue(Grid.ColumnProperty, 2);
            return buttons;
        }
    }
}
