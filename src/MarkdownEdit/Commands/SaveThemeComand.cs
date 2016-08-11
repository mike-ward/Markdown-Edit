using System.Windows;
using System.Windows.Input;

namespace MarkdownEdit.Commands
{
    internal static class SaveThemeComand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static SaveThemeComand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e) => App.UserSettings.Save();
    }
}
