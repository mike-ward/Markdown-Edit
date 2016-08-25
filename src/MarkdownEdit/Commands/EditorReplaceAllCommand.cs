using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class EditorReplaceAllCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static EditorReplaceAllCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            var tuple = (Tuple<Regex, string>)e.Parameter;
            mainWindow.Editor.ReplaceAll(tuple.Item1, tuple.Item2);
        }
    }
}