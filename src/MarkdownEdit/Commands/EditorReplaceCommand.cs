using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Commands
{
    internal static class EditorReplaceCommand
    {
        public static readonly RoutedCommand Command = ApplicationCommands.Replace;

        static EditorReplaceCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            var mainWindow = (MainWindow)sender;
            var tuple = (Tuple<Regex, string>)e.Parameter;
            mainWindow.Editor.Replace(tuple.Item1, tuple.Item2);
        }
    }
}