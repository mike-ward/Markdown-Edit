using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
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

        private bool _canceled;
        private bool _useClipboardImage;
        private bool _uploading;
        private string _documentFileName;

        public ImageDropDialog()
        {
            InitializeComponent();
            Loaded += OnLoaded;
        }

        public string DocumentFileName
        {
            get { return _documentFileName; }
            set { Set(ref _documentFileName, value); }
        }

        public bool Uploading
        {
            get { return _uploading; }
            set { Set(ref _uploading, value); }
        }

        public bool UseClipboardImage
        {
            get { return _useClipboardImage; }
            set { Set(ref _useClipboardImage, value); }
        }

        private void OnLoaded(object sender, EventArgs eventArgs)
        {
            var position = UseClipboardImage
                ? TextEditor.TextArea.TextView.GetVisualPosition(TextEditor.TextArea.Caret.Position, VisualYPosition.LineBottom)
                : DragEventArgs.GetPosition(TextEditor);

            var screen = TextEditor.PointToScreen(new Point(position.X, position.Y));
            Left = screen.X;
            Top = screen.Y;

            var hasDocumentFileName = !string.IsNullOrWhiteSpace(DocumentFileName);
            SetDocumentFoldersMenuItems();
            InsertPathMenuItem.IsEnabled = !UseClipboardImage && hasDocumentFileName;
            AsLocalFileMenuItem.IsEnabled = hasDocumentFileName;
            ContextMenu.Closed += (o, args) => { if (!Uploading) Close(); };
            Dispatcher.InvokeAsync(() => ContextMenu.IsOpen = true);
        }

        private void SetDocumentFoldersMenuItems()
        {
            if (string.IsNullOrWhiteSpace(DocumentFileName)) return;
            var directoryName = Path.GetDirectoryName(DocumentFileName);
            if (string.IsNullOrWhiteSpace(directoryName)) throw new Exception("directoryName in ImageDropDialog member is invalid");

            var documentFolder = new[] {".\\"};

            var folders = Directory.EnumerateDirectories(directoryName)
                .Select(d => "." + d.Remove(0, directoryName.Length));

            AsLocalFileMenuItem.ItemsSource = documentFolder.Concat(folders).ToArray();
        }

        private string[] DroppedFiles()
        {
            var files = DragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            return files;
        }

        private void TryIt(Action<string> action)
        {
            try
            {
                if (UseClipboardImage)
                {
                    action(null);
                }
                else
                {
                    foreach (var droppedFile in DroppedFiles().Where(Images.IsImageUrl))
                    {
                        action(droppedFile);
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Alert(ex.Message);
            }
            finally
            {
                ActivateClose();
            }
        }

        private void OnInsertPath(object sender, RoutedEventArgs e)
        {
            TryIt(droppedFilePath =>
            {
                var relativePath = FileExtensions
                    .MakeRelativePath(DocumentFileName, droppedFilePath)
                    .Replace('\\', '/');

                var file = Path.GetFileNameWithoutExtension(droppedFilePath);
                InsertImageTag(TextEditor, DragEventArgs, relativePath, file);
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

                string name;
                byte[] image;

                if (UseClipboardImage)
                {
                    name = "clipboard";
                    image = Images.ClipboardDibToBitmapSource().ToPngArray();
                }
                else
                {
                    var files = DroppedFiles();
                    if (files.Length > 1) throw new Exception("Upload only 1 file at a time");
                    var path = files[0];
                    name = Path.GetFileNameWithoutExtension(path);
                    image = File.ReadAllBytes(path);
                }

                Uploading = true;
                var link = await new ImageUploadImgur().UploadBytesAsync(image, progress, completed);
                ActivateClose();

                if (Uri.IsWellFormedUriString(link, UriKind.Absolute)) InsertImageTag(TextEditor, DragEventArgs, link, name);
                else Utility.Alert(link);
            }
            catch (Exception ex)
            {
                ActivateClose();
                Utility.Alert(ex.Message);
            }
        }

        private void ActivateClose()
        {
            Owner?.Activate();
            Close();
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
                string title;
                var image = new byte[0];

                if (UseClipboardImage)
                {
                    var name = PromptDialog.Prompt((string)TranslationProvider.Translate("aslocalfile-save-file-as"));
                    if (string.IsNullOrWhiteSpace(name)) return;
                    image = Images.ClipboardDibToBitmapSource().ToPngArray();
                    title = name + ".png";
                }
                else
                {
                    title = Path.GetFileName(droppedFilePath);
                }
                var link = Path.Combine(documentRelativeDestinationPath, title);
                var destination = Path.Combine(Path.GetDirectoryName(DocumentFileName), link);
                if (link.Contains(" ")) link = $"<{link}>";
                if (File.Exists(destination))
                {
                    var message = (string)TranslationProvider.Translate("image-drop-overwrite-file");
                    if (Utility.ConfirmYesNo(message) != MessageBoxResult.Yes) return;
                }

                if (UseClipboardImage) File.WriteAllBytes(destination, image);
                else File.Copy(droppedFilePath, destination, true);

                InsertImageTag(TextEditor, DragEventArgs, link, title);
            });
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            _canceled = true;
            ActivateClose();
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
}