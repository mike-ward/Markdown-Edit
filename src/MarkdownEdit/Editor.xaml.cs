using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public partial class Editor
    {
        private string _textSwap { get; set; }

        public Editor()
        {
            InitializeComponent();
            EditorBox.Loaded += EditorBoxOnLoaded;
        }

        private void EditorBoxOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            LoadFile(Settings.Default.LastOpenFile);
            EditorBox.TextChanged += EditorBoxOnTextChanged;
            EditorBox.Dispatcher.InvokeAsync(() => EditorBoxOnTextChanged(this, null));
        }

        private void EditorBoxOnTextChanged(object sender, TextChangedEventArgs textChangedEventArgs)
        {
            MainWindow.UpdatePreviewCommand.Execute(EditorBox.Text, this);
        }

        public void OpenFileHandler()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            LoadFile(dialog.FileNames[0]);
        }

        public void LoadFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return;
            EditorBox.Text = File.ReadAllText(file);
            Settings.Default.LastOpenFile = file;
        }

        public void ToggleHelp()
        {
            if (_textSwap == null)
            {
                _textSwap = EditorBox.Text;
                EditorBox.Text = Properties.Resources.Help;
                EditorBox.IsReadOnly = true;
            }
            else
            {
                EditorBox.IsReadOnly = false;
                EditorBox.Text = _textSwap;
                _textSwap = null;
            }
        }

        public void WordWrapHandler()
        {
            var wrap = EditorBox.TextWrapping;
            EditorBox.TextWrapping = wrap == TextWrapping.Wrap ? TextWrapping.NoWrap : TextWrapping.Wrap;
        }
    }
}