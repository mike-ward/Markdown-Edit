using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using MarkdownEdit.ImageUpload;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    public partial class ImageDropDialog : INotifyPropertyChanged
    {
        public string FileName { get; set; }
        public TextEditor TextEditor { get; set; }
        public DragEventArgs DragEventArgs { get; set; }

        private bool _canceled;
        private byte[] _image;

        public ImageDropDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public byte[] Image
        {
            get { return _image; }
            set { Set(ref _image, value); }
        }

        private void OnLoaded(object sender, EventArgs eventArgs)
        {
            Point position;
            if (Image == null)
            {
                position = DragEventArgs.GetPosition(TextEditor);
            }
            else
            {
                position = TextEditor.TextArea.TextView.GetVisualPosition(TextEditor.TextArea.Caret.Position, VisualYPosition.LineBottom);
                var clickEvent = new RoutedEventArgs(MenuItem.ClickEvent);
                ImgurMenuItem.RaiseEvent(clickEvent);
            }
            var screen = TextEditor.PointToScreen(new Point(position.X, position.Y));
            Left = screen.X;
            Top = screen.Y;
        }

        private string DroppedFilePath()
        {
            var files = DragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            var path = files?[0];
            return path;
        }

        private void TryIt(Action<string> func)
        {
            try
            {
                var path = DroppedFilePath();
                func(path);
            }
            catch (Exception ex)
            {
                Utility.Alert(ex.Message);
            }
            finally
            {
                Close();
            }
        }

        private void OnInsertPath(object sender, RoutedEventArgs e)
        {
            TryIt(path =>
            {
                var file = Path.GetFileNameWithoutExtension(path);
                path = path.Replace('\\', '/');
                if (path.Contains(" ")) path = $"<{path}>";
                InsertImageTag(TextEditor, DragEventArgs, path, file);
            });
        }

        private async void OnUploadToImgur(object sender, RoutedEventArgs e)
        {
            try
            {
                UploadProgressChangedEventHandler progress = (o, args) => TextEditor.Dispatcher.InvokeAsync(() =>
                {
                    if (_canceled) ((WebClient)o).CancelAsync();
                    var progressPercentage = (int)((args.BytesSent/(double)args.TotalBytesToSend)*100);
                    ProgressBar.Value = progressPercentage;
                    if (progressPercentage == 100) ProgressBar.IsIndeterminate = true;
                });

                UploadValuesCompletedEventHandler completed = (o, args) => { if (_canceled) ((WebClient)o).CancelAsync(); };

                var name = "Clipboard";

                if (Image == null)
                {
                    var path = DroppedFilePath();
                    name = Path.GetFileNameWithoutExtension(path);
                    Image = File.ReadAllBytes(path);
                }

                var link = await new ImageUploadImgur().UploadBytesAsync(Image, progress, completed);
                Close();
                if (Uri.IsWellFormedUriString(link, UriKind.Absolute)) InsertImageTag(TextEditor, DragEventArgs, link, name);
                else Utility.Alert(link);
            }
            catch (Exception ex)
            {
                Close();
                Utility.Alert(ex.Message);
            }
        }

        private void OnInsertDataUri(object sender, RoutedEventArgs e)
        {
            TryIt(path =>
            {
                var dataUri = Images.ImageFileToDataUri(path);
                TextEditor.Document.Insert(GetInsertOffset(TextEditor, DragEventArgs), dataUri);
            });
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        private void OnInsertFile(object sender, RoutedEventArgs e)
        {
            TryIt(path =>
            {
                var destination = Path.Combine(
                    Path.GetDirectoryName(FileName),
                    Path.GetFileName(path));

                File.Copy(path, destination);
                var title = Path.GetFileNameWithoutExtension(destination);
                var link = Path.GetFileName(destination);
                if (link.Contains(" ")) link = $"<{link}>";
                InsertImageTag(TextEditor, DragEventArgs, link, title);
            });
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _canceled = true;
            Close();
        }

        private static void InsertImageTag(TextEditor textEditor, DragEventArgs dragEventArgs, string link, string title)
        {
            textEditor.Document.Insert(GetInsertOffset(textEditor, dragEventArgs), $"![{title}]({link})\n");
        }

        private static int GetInsertOffset(TextEditor textEditor, DragEventArgs dragEventArgs)
        {
            if (dragEventArgs == null) return textEditor.CaretOffset;
            var position = dragEventArgs.GetPosition(textEditor);
            var offset = GetOffsetFromMousePosition(textEditor, position);
            if (offset == -1) offset = textEditor.Document.TextLength;
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

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public sealed class NullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (!(value is Visibility) || (Visibility)value != Visibility.Visible);
        }
    }

    public sealed class NotNullToVisibleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value != null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is Visibility && (Visibility)value == Visibility.Visible);
        }
    }
}