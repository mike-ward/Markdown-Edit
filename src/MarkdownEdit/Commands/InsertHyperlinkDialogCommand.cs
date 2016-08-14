using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class InsertHyperlinkDialogCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static InsertHyperlinkDialogCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute, CanExecute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var editor = ((MainWindow)sender).Editor;
            var dialog = new InsertHyperlinkDialog(editor.EditBox.SelectedText) { Owner = Application.Current.MainWindow };
            dialog.ShowDialog();
        }

        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            var editor = ((MainWindow)sender).Editor;
            e.CanExecute = !editor.EditBox.IsReadOnly;
        }
    }
}