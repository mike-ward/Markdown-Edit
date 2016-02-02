using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using MarkdownEdit.i18n;
using MarkdownEdit.ImageUpload;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    public partial class ImageDropDialog : INotifyPropertyChanged
    {
        public TextEditor TextEditor { get; set; }
        public DragEventArgs DragEventArgs { get; set; }
        public bool UseClipboardImage { get; set; }

        private bool _canceled;
        private object _uploading;
        private string[] _doumentFolders;
        private string _documentFileName;

        public ImageDropDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public string[] DoumentFolders
        {
            get { return _doumentFolders; }
            set { Set(ref _doumentFolders, value); }
        }

        public string DocumentFileName
        {
            get { return _documentFileName; }
            set { Set(ref _documentFileName, value); }
        }

        public object Uploading
        {
            get { return _uploading; }    
            set { Set(ref _uploading, value); }
        }

        private void OnLoaded(object sender, EventArgs eventArgs)
        {
            AsLocalFile.SubmenuOpened += AsLocalFileOnSubmenuOpened;

            var position = UseClipboardImage 
                ? DragEventArgs.GetPosition(TextEditor) 
                : TextEditor.TextArea.TextView.GetVisualPosition(TextEditor.TextArea.Caret.Position, VisualYPosition.LineBottom);

            var screen = TextEditor.PointToScreen(new Point(position.X, position.Y));
            Left = screen.X;
            Top = screen.Y;
        }

        private void AsLocalFileOnSubmenuOpened(object sender, RoutedEventArgs routedEventArgs)
        {
            var directoryName = Path.GetDirectoryName(DocumentFileName);
            if (string.IsNullOrWhiteSpace(directoryName)) throw new Exception("directoryName in ImageDropDialog member is invalid");

            var documentFolder = new[] {".\\"};

            var folders = Directory.EnumerateDirectories(directoryName)
                .Select(d => "." + d.Remove(0, directoryName.Length));

            DoumentFolders = documentFolder.Concat(folders).ToArray();
        }

        private string DroppedFilePath()
        {
            var files = DragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            var path = files?[0];
            return path;
        }

        private void TryIt(Action<string> action)
        {
            try
            {
                var droppedFilePath = UseClipboardImage ? DroppedFilePath() : null;
                action(droppedFilePath);
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
            TryIt(droppedFilePath =>
            {
                var file = Path.GetFileNameWithoutExtension(droppedFilePath);
                droppedFilePath = droppedFilePath.Replace('\\', '/');
                if (droppedFilePath.Contains(" ")) droppedFilePath = $"<{droppedFilePath}>";
                InsertImageTag(TextEditor, DragEventArgs, droppedFilePath, file);
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

                byte[] image;

                if (UseClipboardImage)
                {
                    image = Images.ClipboardDibToBitmapSource().ToPngArray();
                }
                else
                {
                    var path = DroppedFilePath();
                    name = Path.GetFileNameWithoutExtension(path);
                    image = File.ReadAllBytes(path);
                }

                Uploading = image;
                var link = await new ImageUploadImgur().UploadBytesAsync(image, progress, completed);

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
            TryIt(droppedFilePath =>
            {
                var dataUri = UseClipboardImage 
                    ? Images.ClipboardDibToDataUri()
                    : Images.ImageFileToDataUri(droppedFilePath);

                TextEditor.Document.Insert(GetInsertOffset(TextEditor, DragEventArgs), dataUri);
            });
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        private void FolderMenuItemClicked(object sender, RoutedEventArgs e)
        {
            var menuItem = (MenuItem)sender;
            OnInsertFile(menuItem.Header.ToString());
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        private void OnInsertFile(string documentRelativeDestinationPath)
        {
            TryIt(droppedFilePath =>
            {
                var title = Path.GetFileName(droppedFilePath);
                var link = Path.Combine(documentRelativeDestinationPath, title);
                var destination = Path.Combine(Path.GetDirectoryName(DocumentFileName), link);
                if (link.Contains(" ")) link = $"<{link}>";
                if (File.Exists(destination))
                {
                    var message = (string)TranslationProvider.Translate("image-drop-overwrite-file");
                    if (Utility.ConfirmYesNo(message) != MessageBoxResult.Yes) return;
                }
                File.Copy(droppedFilePath, destination, true);
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

    public sealed class NullOrEmptyToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            return !string.IsNullOrWhiteSpace(text);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    public sealed class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}