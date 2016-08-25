using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal static class SaveAsHtmlTemplateCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static SaveAsHtmlTemplateCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            EditorLoadSave.SaveFileAs(mainWindow.Editor, "html-with-template");
        }
    }
}