using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal static class LoadThemeCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static LoadThemeCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e) 
            => App.UserSettings.Theme = e.Parameter as Theme;
    }
}
