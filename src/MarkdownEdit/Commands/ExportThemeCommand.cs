using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using MarkdownEdit.Models;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace MarkdownEdit.Commands
{
    internal static class ExportThemeCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        static ExportThemeCommand()
        {
            Application.Current.MainWindow.CommandBindings.Add(new CommandBinding(Command, Execute));
        }

        private static void Execute(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var filename = GetFileName();
                if (filename == null) return;
                var theme = JsonConvert.SerializeObject(App.UserSettings.Theme);
                // ReSharper disable once AssignNullToNotNullAttribute
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                File.WriteAllText(filename, theme);
            }
            catch (Exception ex)
            {
                Notify.Alert(ex.Message);
            }
        }

        private static string GetFileName()
        {
            var dialog = new SaveFileDialog
            {
                Filter = "json files (*.json)|*.json|"
                         + "All files (*.*)|*.*"
            };

            var showDialog = dialog.ShowDialog();
            return (showDialog != null && (bool)showDialog) ? dialog.FileName : null;
        }
    }
}
