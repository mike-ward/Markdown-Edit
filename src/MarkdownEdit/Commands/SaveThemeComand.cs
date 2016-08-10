using System.Windows.Input;

namespace MarkdownEdit.Commands
{
    internal static class SaveThemeComand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();
        public static readonly CommandBinding Bind = new CommandBinding(Command, Execute, CanExecute);
        private static void Execute(object sender, ExecutedRoutedEventArgs e) => App.UserSettings.Save();
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
    }
}
