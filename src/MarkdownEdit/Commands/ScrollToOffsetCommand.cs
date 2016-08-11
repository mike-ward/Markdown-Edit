using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;
using MarkdownEdit.Models;

namespace MarkdownEdit.Commands
{
    internal static class ScrollToOffsetCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ScrollToOffsetCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter == null) return;
            var textEditor = ((MainWindow)sender).Editor.EditBox;
            EditorUtilities.ScrollToOffset(textEditor, (int)e.Parameter);
        }
    }
}
