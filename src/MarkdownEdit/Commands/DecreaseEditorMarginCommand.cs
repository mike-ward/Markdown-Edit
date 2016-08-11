using System;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal class DecreaseEditorMarginCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();
        public static readonly CommandBinding Bind = new CommandBinding(Command, Execute, CanExecute);
        private static void CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            App.UserSettings.SinglePaneMargin = 
                Math.Min(App.UserSettings.SinglePaneMargin + 1, MainWindow.EditorMarginMax);
        }
    }
}
