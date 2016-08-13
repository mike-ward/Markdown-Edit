using System;
using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class InsertHeaderCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static InsertHeaderCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            mainWindow.Editor.InsertHeader(Convert.ToInt32(e.Parameter));
        }
    }
}