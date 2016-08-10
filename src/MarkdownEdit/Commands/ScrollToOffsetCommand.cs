using System.Windows.Input;
using MarkdownEdit.Controls;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal class ScrollToOffsetCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();
        public static readonly CommandBinding Bind = new CommandBinding(Command, Execute, CanExecute);
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter == null) return;
            var textEditor = ((MainWindow)sender).Editor.EditBox;
            EditorUtilities.ScrollToOffset(textEditor, (int)e.Parameter);
        }
    }
}
