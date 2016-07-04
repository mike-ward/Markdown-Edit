using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Rendering;
using MarkdownEdit.i18n;
using MarkdownEdit.ImageUpload;
using MarkdownEdit.Models;
using Microsoft.Win32;

namespace MarkdownEdit.Controls
{
    public partial class ImageDropDialog
    {
        private readonly ImageDropDialogViewModel _vm;

        private bool _canceled;

        public ImageDropDialog()
        {
            InitializeComponent();
            _vm = (ImageDropDialogViewModel)DataContext;
            Loaded += OnLoaded;
        }

        public TextEditor TextEditor { get; set; }
        public DragEventArgs DragEventArgs { get; set; }

        public string DocumentFileName
        {
            set { _vm.DocumentFileName = value; }
        }

        public bool UseClipboardImage
        {
            set { _vm.UseClipboardImage = value; }
        }

        private void OnLoaded(object sender, EventArgs eventArgs)
        {
            if (App.UserSettings.InsertImagePathsOnly && !string.IsNullOrWhiteSpace(_vm.DocumentFileName))
            {
                OnInsertPath(null, null);
                return;
            }

            var position = _vm.UseClipboardImage
                ? TextEditor.TextArea.TextView.GetVisualPosition(TextEditor.TextArea.Caret.Position,
                    VisualYPosition.LineBottom)
                : DragEventArgs.GetPosition(TextEditor);

            var screen = TextEditor.PointToScreen(new Point(position.X, position.Y));
            Left = screen.X;
            Top = screen.Y;

            var hasDocumentFileName = !string.IsNullOrWhiteSpace(_vm.DocumentFileName);
            InsertPathMenuItem.IsEnabled = !_vm.UseClipboardImage && hasDocumentFileName;
            SaveAsItem.IsEnabled = hasDocumentFileName;
            ContextMenu.Closed += (o, args) =>
            {
                if (!_vm.Uploading) Close();
            };
            Dispatcher.InvokeAsync(() => ContextMenu.IsOpen = true);
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
                if (_vm.UseClipboardImage)
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
                Notify.Alert(ex.Message);
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
                    .MakeRelativePath(_vm.DocumentFileName, droppedFilePath)
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
                    var progressPercentage = (int)(args.BytesSent/(double)args.TotalBytesToSend*100);
                    ProgressBar.Value = progressPercentage;
                    if (progressPercentage == 100) ProgressBar.IsIndeterminate = true;
                });

                UploadValuesCompletedEventHandler completed = (o, args) =>
                {
                    if (_canceled) ((WebClient)o).CancelAsync();
                };

                string name;
                byte[] image;

                if (_vm.UseClipboardImage)
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

                _vm.Uploading = true;
                ContextMenu.IsOpen = false;
                var link = await new ImageUploadImgur().UploadBytesAsync(image, progress, completed);
                ActivateClose();

                if (Uri.IsWellFormedUriString(link, UriKind.Absolute))
                    InsertImageTag(TextEditor, DragEventArgs, link, name);
                else Notify.Alert(link);
            }
            catch (Exception ex)
            {
                ActivateClose();
                Notify.Alert(ex.Message);
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
                var dataUri = _vm.UseClipboardImage
                    ? Images.ClipboardDibToDataUri()
                    : Images.ImageFileToDataUri(droppedFilePath);

                TextEditor.Document.Insert(GetInsertOffset(TextEditor, DragEventArgs), dataUri);
            });
        }

        [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
        public void OnSaveAs(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_vm.DocumentFileName)) return;
            Close();

            // Yep, you're reading this right. SaveFileDialog.ShowDialog exits immediately
            // unless I first close this window. Something to do with nested ShowDialog()
            // calls I suspect.

            Application.Current.Dispatcher.InvokeAsync(() =>
                TryIt(droppedFilePath =>
                {
                    var dialog = new SaveFileDialog
                    {
                        OverwritePrompt = true,
                        RestoreDirectory = true,
                        Filter = "All files (*.*)|*.*"
                    };

                    if (dialog.ShowDialog() == false) return;
                    if (string.IsNullOrEmpty(dialog.FileName)) return;
                    var fileName = dialog.FileName;

                    string title;
                    var image = new byte[0];

                    if (_vm.UseClipboardImage)
                    {
                        image = Images.ClipboardDibToBitmapSource().ToPngArray();
                        title = fileName + ".png";
                    }
                    else
                    {
                        title = Path.GetFileName(droppedFilePath);
                    }

                    if (File.Exists(fileName))
                    {
                        var message = (string)TranslationProvider.Translate("image-drop-overwrite-file");
                        if (Notify.ConfirmYesNo(message) != MessageBoxResult.Yes) return;
                    }

                    if (_vm.UseClipboardImage) File.WriteAllBytes(fileName, image);
                    else File.Copy(droppedFilePath, fileName, true);

                    var link = FileExtensions.MakeRelativePath(_vm.DocumentFileName, fileName);
                    InsertImageTag(TextEditor, DragEventArgs, link, title);
                }));
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
    }
}