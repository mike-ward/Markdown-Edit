using System.Windows.Input;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal static class LoadThemeCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();
        public static readonly CommandBinding Bind = new CommandBinding(Command, Execute, CanExecute);
        private static void Execute(object sender, ExecutedRoutedEventArgs e) => App.UserSettings.Theme = e.Parameter as Theme;
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
    }
}
