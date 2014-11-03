using System.IO;
using System.Linq;
using System.Windows.Controls;
using Newtonsoft.Json;

namespace MarkdownEdit
{
    public partial class ThemeDialog
    {
        public ThemeDialog()
        {
            InitializeComponent();
            var assemblyFolder = Utility.AssemblyFolder();
            var path = Path.Combine(assemblyFolder, "Themes");
            var files = Directory.EnumerateFiles(path, "*.json");
            ThemeListBox.ItemsSource = files
                .Select(f => JsonConvert.DeserializeObject<Theme>(File.ReadAllText(f)))
                .Select(t => new ListBoxItem {Tag = t, Content = t.Name});
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainWindow.LoadThemeCommand.Execute(((ListBoxItem)ThemeListBox.SelectedItem).Tag, Owner);
        }
    }
}