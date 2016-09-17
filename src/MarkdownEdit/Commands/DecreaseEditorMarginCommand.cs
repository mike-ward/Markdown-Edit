using System;
using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class DecreaseEditorMarginCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static DecreaseEditorMarginCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            App.UserSettings.SinglePaneMargin = 
                Math.Min(App.UserSettings.SinglePaneMargin + 1, MainWindowViewModel.EditorMarginMax);
        }
    }
}
