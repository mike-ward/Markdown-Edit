using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class InsertHyperlinkCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static InsertHyperlinkCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter == null) return;
            var textEditor = ((MainWindow)sender).Editor.EditBox;
            Editor.InsertHyperlinkCommand.Execute(e.Parameter, textEditor);
        }
    }
}
