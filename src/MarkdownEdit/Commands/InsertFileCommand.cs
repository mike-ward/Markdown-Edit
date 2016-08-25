using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal static class InsertFileCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static InsertFileCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            EditorLoadSave.InsertFile(mainWindow.Editor, null);
        }
    }
}
