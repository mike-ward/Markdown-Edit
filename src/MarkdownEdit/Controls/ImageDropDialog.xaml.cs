using System;
using System.IO;
using System.Net;
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
            var file = Path.GetFileNameWithoutExtension(files[0]);
            var path = files[0].Replace('\\', '/');
            TextEditor.Document.Insert(TextEditor.CaretOffset, $"![{file}]({path}\n)");
            Close();
        }

        private void OnUploadToImgur(object sender, RoutedEventArgs e)
        {
            var files = DragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            if (files == null) return;
            var path = files[0];
            var name = Path.GetFileNameWithoutExtension(path);

            Func<long, long, int> percentSent = (s, t) => (int)(((double)s / t) * 100);

            UploadProgressChangedEventHandler progress = (o, args) => TextEditor.Dispatcher.InvokeAsync(() =>
                ImgurMenuItem.Header = $"{percentSent(args.BytesSent, args.TotalBytesToSend)}%");

            UploadValuesCompletedEventHandler completed = (o, args) => TextEditor.Dispatcher.InvokeAsync(Close);

            new ImageUploadImgur()
                .UploadBytesAsync(File.ReadAllBytes(path), progress, completed)
                .ContinueWith(task => TextEditor.Dispatcher.InvokeAsync(() =>
                    TextEditor.Document.Insert(TextEditor.CaretOffset, string.Format("![{1}]({0})\n", task.Result, name))));
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}