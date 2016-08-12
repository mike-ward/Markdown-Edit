using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;
using Clipboard = MarkdownEdit.Models.Clipboard;

namespace MarkdownEdit.Commands
{
    internal static class ExportHtmlTemplateCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ExportHtmlTemplateCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            Clipboard.ExportHtmlToClipboard(mainWindow.Editor.Text, true);
        }
    }
}