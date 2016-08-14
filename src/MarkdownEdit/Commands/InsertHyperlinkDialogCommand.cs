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
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var editor = ((MainWindow)sender).Editor;

            editor.IfNotReadOnly(() =>
            {
                var dialog = new InsertHyperlinkDialog(editor.EditBox.SelectedText) {Owner = Application.Current.MainWindow};
                dialog.ShowDialog();
            });
        }
    }
}