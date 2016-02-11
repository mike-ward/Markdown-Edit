using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;

namespace MarkdownEdit.Controls
{
    public class RecentFile
    {
        public string FileName { get; set; }

        public string DisplayName { get; set; }

        public override string ToString()
        {
            return DisplayName;
        }
    }

    public partial class RecentFilesDialog
    {
        public RecentFilesDialog()
        {
            InitializeComponent();
            IsVisibleChanged += OnIsVisibleChanged;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            IsVisibleChanged -= OnIsVisibleChanged;
            FilesListBox.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
            SetItemsSource();
        }

        private void SetItemsSource()
        {
            var kb = new[] {'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j'};
            FilesListBox.ItemsSource = Settings.Default.RecentFiles?.Cast<string>()
                .Select((f, i) => new RecentFile
                {
                    FileName = f,
                    DisplayName = $"{kb[i%kb.Length]}: {f.StripOffsetFromFileName()}"
                }) ?? new RecentFile[0];
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
            OpenFile();
        }

        private void OpenFile()
        {
            var file = FilesListBox.SelectedItem as RecentFile;
            if (file == null) return;
            ApplicationCommands.Open.Execute(file.FileName, Application.Current.MainWindow);
            ApplicationCommands.Close.Execute(null, this);
        }

        private void ClearOnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.RecentFiles = new StringCollection();
            FilesListBox.ItemsSource = new RecentFile[0];
            Close();
        }

        public static void UpdateRecentFiles(string file, int offset = 0)
        {
            if (string.IsNullOrWhiteSpace(file)) return;
            var recent = Settings.Default.RecentFiles ?? new StringCollection();

            var files = recent
                .Cast<string>()
                .Where(f => f.StripOffsetFromFileName() != file);

            var sc = new StringCollection(); // string collections suck
            sc.AddRange(files.Take(19).ToArray());
            sc.Insert(0, file.AddOffsetToFileName(offset));
            Settings.Default.RecentFiles = sc;
        }

        private void RemoveFromRecentFiles(object sender, ExecutedRoutedEventArgs e)
        {
            var recentFile = e.Parameter as RecentFile;
            if (recentFile == null) return;
            var recent = Settings.Default.RecentFiles ?? new StringCollection();
            recent.Remove(recentFile.FileName);
            SetItemsSource();
        }

        private void OpenFileCommandHander(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFile();
        }
    }
}