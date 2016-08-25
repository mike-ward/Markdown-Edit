using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Properties;

namespace MarkdownEdit.Commands
{
    internal static class ToggleAutoSaveCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ToggleAutoSaveCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            Settings.Default.AutoSave = !Settings.Default.AutoSave;
        }
    }
}