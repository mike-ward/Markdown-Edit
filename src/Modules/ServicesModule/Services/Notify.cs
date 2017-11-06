using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Infrastructure;
using MahApps.Metro.SimpleChildWindow;

namespace ServicesModule.Services
{
    public class Notify : INotify
    {
        private readonly double _fontSize = 18;
        private readonly double _buttonWidth = 100;
        private readonly double _buttonColumnWidth = 120;
        private readonly Thickness _buttonThickness = new Thickness(0, 10, 0, 0);

        private static ChildWindow Dialog()
        {
            var grid = new Grid { Margin = new Thickness(10), MinHeight = 75 ,Width = 500 };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

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
                Title = Constants.ProgramName,
                TitleBarBackground = (Brush)Application.Current.MainWindow?.FindResource("GrayBrush2")
            };

            return dialog;
        }

        public async Task<bool> Alert(string alert)
        {
            var dialog = Dialog();
            var content = (Grid)dialog.Content;
            var message = new TextBlock { Text = alert, FontSize = _fontSize };
            message.SetValue(Grid.RowProperty, 0);

            var buttons = new Grid { Margin = _buttonThickness};
            buttons.SetValue(Grid.RowProperty, 2);
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });

            var ok = new Button { Content = new TextBlock { Text = "OK" }, Width = _buttonWidth };
            ok.SetValue(Grid.RowProperty, 2);
            ok.SetValue(Grid.ColumnProperty, 1);
            ok.Click += (sd, ea) => dialog.Close();

            buttons.Children.Add(ok);

            content.Children.Add(message);
            content.Children.Add(buttons);

            await Application.Current.MainWindow.ShowChildWindowAsync(dialog);
            return true;
        }

        public async Task<MessageBoxResult> ConfirmYesNo(string question)
        {
            var result = MessageBoxResult.No;
            var dialog = Dialog();
            var content = (Grid)dialog.Content;
            var message = new TextBlock { Text = question, FontSize = _fontSize };
            message.SetValue(Grid.RowProperty, 0);

            var buttons = new Grid { Margin = _buttonThickness };
            buttons.SetValue(Grid.RowProperty, 2);
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });

            var yes = new Button { Content = new Label { Content = "_Yes" }, Width = _buttonWidth };
            yes.SetValue(Grid.RowProperty, 2);
            yes.SetValue(Grid.ColumnProperty, 1);
            yes.Click += (sd, ea) =>
            {
                result = MessageBoxResult.Yes;
                dialog.Close();
            };
            var no = new Button { Content = new Label { Content = "_No" }, Width = _buttonWidth };
            no.SetValue(Grid.RowProperty, 2);
            no.SetValue(Grid.ColumnProperty, 2);
            no.Click += (sd, ea) =>
            {
                result = MessageBoxResult.No;
                dialog.Close();
            };

            buttons.Children.Add(yes);
            buttons.Children.Add(no);

            content.Children.Add(message);
            content.Children.Add(buttons);

            await Application.Current.MainWindow.ShowChildWindowAsync(dialog);
            return result;
        }

        public async Task<MessageBoxResult> ConfirmYesNoCancel(string question)
        {
            var result = MessageBoxResult.Cancel;
            var dialog = Dialog();
            var content = (Grid)dialog.Content;
            var message = new TextBlock { Text = question, FontSize = _fontSize };
            message.SetValue(Grid.RowProperty, 0);

            var buttons = new Grid { Margin = _buttonThickness };
            buttons.SetValue(Grid.RowProperty, 2);
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });
            buttons.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(_buttonColumnWidth) });

            var yes = new Button { Content = new Label { Content = "_Yes" }, Width = _buttonWidth };
            yes.SetValue(Grid.RowProperty, 2);
            yes.SetValue(Grid.ColumnProperty, 1);
            yes.Click += (sd, ea) =>
            {
                result = MessageBoxResult.Yes;
                dialog.Close();
            };
            var no = new Button { Content = new Label { Content = "_No" }, Width = _buttonWidth };
            no.SetValue(Grid.RowProperty, 2);
            no.SetValue(Grid.ColumnProperty, 2);
            no.Click += (sd, ea) =>
            {
                result = MessageBoxResult.No;
                dialog.Close();
            };
            var cancel = new Button { Content = new Label { Content = "_Cancel" }, Width = _buttonWidth };
            cancel.SetValue(Grid.ColumnProperty, 3);
            cancel.Click += (sd, ea) =>
            {
                result = MessageBoxResult.Cancel;
                dialog.Close();
            };

            buttons.Children.Add(yes);
            buttons.Children.Add(no);
            buttons.Children.Add(cancel);

            content.Children.Add(message);
            content.Children.Add(buttons);

            await Application.Current.MainWindow.ShowChildWindowAsync(dialog);
            return result;
        }
    }
}
