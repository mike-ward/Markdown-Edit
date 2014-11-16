using System.Windows.Input;

namespace MarkdownEdit
{

    public partial class ImportFromDialog
    {
        public static RoutedCommand ShowProviderFilePickerCommand = new RoutedCommand();

        public ImportFromDialog()
        {
            InitializeComponent();
        }

        private void ExecuteShowProviderFilePicker(object sender, ExecutedRoutedEventArgs e)
        {
            var remoteProvider = (IRemoteProvider)e.Parameter;
            remoteProvider.OpenFilePickerAsync();
        }
    }
}
