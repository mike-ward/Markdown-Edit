using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class PasteFromHtmlCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static PasteFromHtmlCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute, CanExecute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var editor = ((MainWindow)sender).Editor;
            try
            {
                editor.ConvertFromHtml = true;
                editor.EditBox.Paste();
            }
            finally
            {
                editor.ConvertFromHtml = false;
            }
        }

        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var editor = ((MainWindow)sender).Editor;
            e.CanExecute = !editor.EditBox.IsReadOnly;
        }
    }
}