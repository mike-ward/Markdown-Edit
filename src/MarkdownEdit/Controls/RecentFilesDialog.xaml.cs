using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public partial class RecentFilesDialog
    {
        public RecentFilesDialog()
        {
            InitializeComponent();
            var kb = new[] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j'};
            FilesListBox.ItemsSource = Settings.Default.RecentFiles?.Cast<string>().Select((f, i) => string.Format("{0}: {1}", kb[i % kb.Length], f)) ?? new string[0];
            FilesListBox.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
        }

        private void ItemContainerGeneratorOnStatusChanged(object sender, EventArgs eventArgs)
        {
            if (FilesListBox.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                var index = FilesListBox.SelectedIndex;
                if (index >= 0)
                {
                    var item = FilesListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    item?.Focus();
                }
            }
        }

        public static void Display(Window owner)
        {
            var dialog = new RecentFilesDialog {Owner = owner};
            dialog.ShowDialog();
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }

        private void OnOpen(object sender, RoutedEventArgs e)
        {
            var file = FilesListBox.SelectedItem as string;
            if (string.IsNullOrWhiteSpace(file)) return;
            ApplicationCommands.Open.Execute(file.Substring(3), Application.Current.MainWindow);
            ApplicationCommands.Close.Execute(null, this);
        }

        private void ClearOnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.RecentFiles = new StringCollection();
            FilesListBox.ItemsSource = new string[0];
            Close();
        }

        public static void UpdateRecentFiles(string file)
        {
            var recent = Settings.Default.RecentFiles ?? new StringCollection();
            recent.Remove(file);
            recent.Insert(0, file);
            var sc = new StringCollection(); // string collections suck
            sc.AddRange(recent.Cast<string>().Take(20).ToArray());
            Settings.Default.RecentFiles = sc;
        }
    }
}