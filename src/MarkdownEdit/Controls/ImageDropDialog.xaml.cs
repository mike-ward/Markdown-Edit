using System;
using System.IO;
using System.Windows;
using ICSharpCode.AvalonEdit;
using MarkdownEdit.ImageUpload;

namespace MarkdownEdit.Controls
{
    public partial class ImageDropDialog
    {
        public TextEditor TextEditor { get; set; }
        public DragEventArgs DragEventArgs { get; set; }

        public ImageDropDialog()
        {
            InitializeComponent();
            Activated += OnActivated;
        }

        private void OnActivated(object sender, EventArgs eventArgs)
        {
            var position = DragEventArgs.GetPosition(TextEditor);
            var screen = new Point(position.X, position.Y);
            Left = screen.X;
            Top = screen.Y;
        }

        private void OnInsertPath(object sender, RoutedEventArgs e)
        {
            var files = DragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null) return;
            var file = Path.GetFileName(files[0]);
            var path = files[0].Replace('\\', '/');
            var position = DragEventArgs.GetPosition(TextEditor);
            var offset = GetOffsetFromMousePosition(TextEditor, position);
            if (offset == -1) offset = TextEditor.Document.TextLength;
            TextEditor.Document.Insert(offset, string.Format("![{0}]({1}) ", file, path));
        }

        private static int GetOffsetFromMousePosition(TextEditor textEditor, Point positionRelativeToTextView)
        {
            var textView = textEditor.TextArea.TextView;
            var pos = positionRelativeToTextView;
            if (pos.Y < 0) pos.Y = 0;
            if (pos.Y > textView.ActualHeight) pos.Y = textView.ActualHeight;
            pos += textView.ScrollOffset;
            if (pos.Y > textView.DocumentHeight) pos.Y = textView.DocumentHeight;
            var line = textView.GetVisualLineFromVisualTop(pos.Y);
            if (line == null) return -1;
            var visualColumn = line.GetVisualColumn(pos);
            return line.GetRelativeOffset(visualColumn) + line.FirstDocumentLine.Offset;
        }

        private void OnUploadToImgur(object sender, RoutedEventArgs e)
        {
            var files = DragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null) return;
            var path = files[0];

            var loader = new ImageUploadImgur();
            loader
                .UploadBytesAsync(File.ReadAllBytes(path))
                .ContinueWith(task => TextEditor.Dispatcher.InvokeAsync(
                    () => TextEditor.Document.Insert(TextEditor.CaretOffset,
                        string.Format("![{1}]({0})", task.Result, Path.GetFileName(path)))));
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}