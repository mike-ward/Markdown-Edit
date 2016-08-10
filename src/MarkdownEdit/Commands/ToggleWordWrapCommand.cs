using System.Windows.Input;
using MarkdownEdit.Properties;

namespace MarkdownEdit.Commands
{
    internal static class ToggleWordWrapCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();
        public static readonly CommandBinding Bind = new CommandBinding(Command, Execute, CanExecute);
        private static void Execute(object sender, ExecutedRoutedEventArgs e) => Settings.Default.WordWrapEnabled = !Settings.Default.WordWrapEnabled;
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;
    }
}
