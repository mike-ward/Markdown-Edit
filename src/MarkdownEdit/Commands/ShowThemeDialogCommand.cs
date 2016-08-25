using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class ShowThemeDialogCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ShowThemeDialogCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            new ThemeDialog { Owner = (MainWindow)sender, CurrentTheme = App.UserSettings.Theme }.ShowDialog();
        }
    }
}