using System;
using System.IO;
using System.Net;
using System.Windows;
using ICSharpCode.AvalonEdit;
using MarkdownEdit.ImageUpload;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    public partial class ImageDropDialog
    {
        public string FileName { get; set; }
        public TextEditor TextEditor { get; set; }
        public DragEventArgs DragEventArgs { get; set; }
        private bool _canceled;

        public ImageDropDialog()
        {
            InitializeComponent();
            Activated += OnActivated;
        }

        private void OnActivated(object sender, EventArgs eventArgs)
        {
            var position = DragEventArgs.GetPosition(TextEditor);
            var screen = TextEditor.PointToScreen(new Point(position.X, position.Y));
            Left = screen.X;
            Top = screen.Y;
        }

        private void OnInsertPath(object sender, RoutedEventArgs e)
        {
            var files = DragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null) return;
            var path = files[0];
            var file = Path.GetFileNameWithoutExtension(path);
            path = path.Replace('\\', '/');
            if (path.Contains(" ")) path = $"<{path}>";
            InsertImageTag(path, file);
            Close();
        }

        private void OnUploadToImgur(object sender, RoutedEventArgs e)
        {
            var files = DragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null) return;
            var path = files[0];
            var name = Path.GetFileNameWithoutExtension(path);

            UploadProgressChangedEventHandler progress = (o, args) => TextEditor.Dispatcher.InvokeAsync(() =>
            {
                if (_canceled) ((WebClient)o).CancelAsync();
                var progressPercentage = (int)((args.BytesSent / (double)args.TotalBytesToSend) * 100);
                ImgurMenuItem.Header = (progressPercentage == 100) ? "Processing" : $"{progressPercentage}%";
            });

            UploadValuesCompletedEventHandler completed = (o, args) => { if (_canceled) ((WebClient)o).CancelAsync(); };

            Action<string, string> processResult = (link, description) =>
            {
                if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
                {
                    InsertImageTag(link, description);
                }
                else
                {
                    MessageBox.Show(link, App.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };

            new ImageUploadImgur()
                .UploadBytesAsync(File.ReadAllBytes(path), progress, completed)
                .ContinueWith(task => TextEditor.Dispatcher.InvokeAsync(() => processResult(task.Result, name)))
                .ContinueWith(task => Dispatcher.InvokeAsync(Close));
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _canceled = true;
            Close();
        }

        private void InsertImageTag(string link, string description)
        {
            TextEditor.Document.Insert(GetInsertOffset(), $"![{description}]({link})\n");
        }

        private int GetInsertOffset()
        {
            var position = DragEventArgs.GetPosition(TextEditor);
            var offset = GetOffsetFromMousePosition(TextEditor, position);
            if (offset == -1) offset = TextEditor.Document.TextLength;
            return offset;
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
    }
}