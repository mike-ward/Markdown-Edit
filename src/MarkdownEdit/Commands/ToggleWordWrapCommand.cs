using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Properties;

namespace MarkdownEdit.Commands
{
    internal static class ToggleWordWrapCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ToggleWordWrapCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e) => 
            Settings.Default.WordWrapEnabled = !Settings.Default.WordWrapEnabled;
    }
}
