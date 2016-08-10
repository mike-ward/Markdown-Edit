using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal class InsertHyperlinkCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();
        public static readonly CommandBinding Bind = new CommandBinding(Command, Execute, CanExecute);
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter == null) return;
            var textEditor = ((MainWindow)sender).Editor.EditBox;
            Editor.InsertHyperlinkCommand.Execute(e.Parameter, textEditor);
        }
    }
}
