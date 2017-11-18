using System.Windows;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;

namespace EditModule.Views
{
    public partial class ImageDropDialog
    {
        private ImageDropDialogViewModel ViewModel => (ImageDropDialogViewModel)DataContext;

        public ImageDropDialog(TextEditor textEditor, DragEventArgs dea)
        {
            InitializeComponent();
            Loaded += (sd, ea) => OnLoad(textEditor, dea);
        }

        private void OnLoad(TextEditor textEditor, DragEventArgs dea)
        {
            ViewModel.TextEditor = textEditor;
            ViewModel.DragEventArgs = dea;
            ContextMenu = ViewModel.CreateContextMenu();

            ViewModel.CloseAction = () =>
            {
                if (ContextMenu != null) ContextMenu.IsOpen = false;
                Close();
            };
        }
    }
}
