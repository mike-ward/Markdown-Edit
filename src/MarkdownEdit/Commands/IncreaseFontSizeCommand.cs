using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class IncreaseFontSizeCommand
    {
        public static readonly RoutedCommand Command = EditingCommands.IncreaseFontSize;

        static IncreaseFontSizeCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            mainWindow.Editor.IncreaseFontSize();
        }
    }
}