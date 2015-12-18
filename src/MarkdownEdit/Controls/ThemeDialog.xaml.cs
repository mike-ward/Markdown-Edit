using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using MarkdownEdit.Models;
using Newtonsoft.Json;

namespace MarkdownEdit.Controls
{
    public partial class ThemeDialog
    {
        private bool _saved;

        public Theme CurrentTheme { get; set; }

        public ThemeDialog()
        {
            InitializeComponent();
            IsVisibleChanged += OnIsVisibleChanged;
            Closed += OnClosed;
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            IsVisibleChanged -= OnIsVisibleChanged;
            var assemblyFolder = Utility.AssemblyFolder();
            var path = Path.Combine(assemblyFolder, "Themes");
            var files = Directory.EnumerateFiles(path, "*.json");
            ThemeListBox.ItemsSource = files
                .Select(LoadTheme)
                .Select(t => new ListBoxItem { Tag = t, Content = t?.Name ?? "Not Loaded" });

            ThemeListBox.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
            var theme = ThemeListBox.Items.Cast<ListBoxItem>().FirstOrDefault(li => ((Theme)li.Tag).Name == CurrentTheme.Name);
            if (theme != null) ThemeListBox.SelectedItem = theme;
        }

        private static Theme LoadTheme(string file)
        {
            try
            {
                return JsonConvert.DeserializeObject<Theme>(File.ReadAllText(file));
            }
            catch (Exception ex)
            {
                Utility.Alert($"{ex.Message} in {file ?? "null"}");
                return null;
            }
        }

        private void OnClosed(object sender, EventArgs eventArgs)
        {
            if (_saved == false) MainWindow.LoadThemeCommand.Execute(CurrentTheme, Owner);
        }

        private void ItemContainerGeneratorOnStatusChanged(object sender, EventArgs eventArgs)
        {
            if (ThemeListBox.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                var index = ThemeListBox.SelectedIndex;
                if (index >= 0)
                {
                    var item = ThemeListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    item?.Focus();
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var theme = ((ListBoxItem)ThemeListBox.SelectedItem).Tag;
            if (theme != null) MainWindow.LoadThemeCommand.Execute(theme, Owner);
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            MainWindow.SaveThemeCommand.Execute(null, Owner);
            _saved = true;
            Close();
        }
    }
}