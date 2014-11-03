using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Newtonsoft.Json;

namespace MarkdownEdit
{
    public partial class ThemeDialog
    {
        public Theme CurrentTheme { get; set; }

        public ThemeDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            var assemblyFolder = Utility.AssemblyFolder();
            var path = Path.Combine(assemblyFolder, "Themes");
            var files = Directory.EnumerateFiles(path, "*.json");
            ThemeListBox.ItemsSource = files
                .Select(f => JsonConvert.DeserializeObject<Theme>(File.ReadAllText(f)))
                .Select(t => new ListBoxItem { Tag = t, Content = t.Name });

            ThemeListBox.ItemContainerGenerator.StatusChanged += ItemContainerGeneratorOnStatusChanged;
            var theme = ThemeListBox.Items.Cast<ListBoxItem>().FirstOrDefault(li => ((Theme)li.Tag).Name == CurrentTheme.Name);
            if (theme != null) ThemeListBox.SelectedItem = theme;
        }

        private void ItemContainerGeneratorOnStatusChanged(object sender, EventArgs eventArgs)
        {
            if (ThemeListBox.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)
            {
                var index = ThemeListBox.SelectedIndex;
                if (index >= 0)
                {
                    var item = ThemeListBox.ItemContainerGenerator.ContainerFromIndex(index) as ListBoxItem;
                    if (item != null) item.Focus();
                }
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow.LoadThemeCommand.Execute(((ListBoxItem)ThemeListBox.SelectedItem).Tag, Owner);
        }

        private void ExecuteClose(object sender, ExecutedRoutedEventArgs e)
        {
            e.Handled = true;
            Close();
        }
    }
}