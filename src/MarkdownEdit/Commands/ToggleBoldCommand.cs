using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class ToggleBoldCommand
    {
        public static readonly RoutedCommand Command = EditingCommands.ToggleBold;

        static ToggleBoldCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            mainWindow.Editor.Bold();
        }
    }
}