using System;
using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class IncreaseEditorMarginCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static  IncreaseEditorMarginCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            App.UserSettings.SinglePaneMargin =
                Math.Max(App.UserSettings.SinglePaneMargin - 1, MainWindow.EditorMarginMin);
        }
    }
}
