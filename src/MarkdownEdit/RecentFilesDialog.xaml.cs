using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public partial class RecentFilesDialog
    {
        public RecentFilesDialog()
        {
            InitializeComponent();
            FilesListBox.ItemsSource = Settings.Default.RecentFiles.Cast<string>().Skip(1);
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
                    if (item != null) item.Focus();
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
            ApplicationCommands.Open.Execute(file, Application.Current.MainWindow);
            ApplicationCommands.Close.Execute(null, this);
        }

        private void ClearOnClick(object sender, RoutedEventArgs e)
        {
            Settings.Default.RecentFiles = new StringCollection();
            FilesListBox.ItemsSource = new string[0];
            Close();
        }
    }

    internal class ItemsControlContainerConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = (ItemsControl)value;
            return items.ItemContainerGenerator.ContainerFromIndex(System.Convert.ToInt32(parameter));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}