using System.Windows;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;

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
            ViewModel.ProgressBar = ProgressBar;
            ContextMenu = ViewModel.CreateContextMenu();

            ViewModel.CloseAction = () =>
            {
                if (ContextMenu != null) ContextMenu.IsOpen = false;
                Close();
            };

            var position = dea
                ?.GetPosition(textEditor) 
                ?? textEditor.TextArea.TextView.GetVisualPosition(textEditor.TextArea.Caret.Position, VisualYPosition.LineBottom);

            var screen = textEditor.PointToScreen(new Point(position.X, position.Y));
            Left = screen.X;
            Top = screen.Y;
        }

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.CancelUpload = true;
        }

        public bool UseClipboard
        {
            get => ViewModel.UseClipboard;
            set => ViewModel.UseClipboard = value;
        }
    }
}
