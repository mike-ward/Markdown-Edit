using System.Windows.Controls;
using MarkdownEdit.Commands;

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
            UpdatePreviewCommand.Command.Execute(EditorBox.Text, this);
        }
    }
}