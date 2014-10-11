using System.IO;
using System.Windows.Controls;
using System.Windows.Forms;

namespace MarkdownEdit
{
    public partial class Editor
    {
        public Editor()
        {
            InitializeComponent();
            EditorBox.TextChanged += EditorBoxOnTextChanged;
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
            using (var reader = new StreamReader(dialog.OpenFile())) EditorBox.Text = reader.ReadToEnd();
        }
    }
}