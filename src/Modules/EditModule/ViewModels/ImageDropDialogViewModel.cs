using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using MahApps.Metro.IconPacks;
using Prism.Mvvm;

namespace EditModule.ViewModels
{
    public class ImageDropDialogViewModel : BindableBase
    {
        private readonly IImageService _imageService;
        private readonly INotify _notify;
        private bool _uploading;
        private ContextMenu _contextMenu;

        public TextEditor TextEditor { get; set; }
        public DragEventArgs DragEventArgs { get; set; }
        public bool UseClipboard { get; set; }
        public ProgressBar ProgressBar { get; set; }
        public bool CancelUpload { get; set; }
        public bool UseClipboardImage { get; set; }
        public Action CloseAction { get; set; }

        public ImageDropDialogViewModel(IImageService imageService, INotify notify)
        {
            _imageService = imageService;
            _notify = notify;
        }

        public bool Uploading
        {
            get => _uploading;
            set => SetProperty(ref _uploading, value);
        }

        public ContextMenu CreateContextMenu()
        {
            _contextMenu = new ContextMenu { IsOpen = true, StaysOpen = true };
            var items = _contextMenu.Items;
            var hasDocumentName = !string.IsNullOrWhiteSpace(TextEditor.Document.FileName);

            // todo: localize
            items.Add(MenuFactory("Insert Path", PackIconMaterialKind.Image, OnInsertPath, hasDocumentName));
            items.Add(MenuFactory("Upload to Imgur", PackIconMaterialKind.CloudUpload, OnUploadToImgur));
            items.Add(MenuFactory("As Data URI", PackIconMaterialKind.Xml, OnInsertDataUri));
            items.Add(MenuFactory("Save As", PackIconMaterialKind.FileDocument, SaveAs, hasDocumentName));
            items.Add(new Separator());
            items.Add(MenuFactory("Cancel", PackIconMaterialKind.Close, OnClose));

            _contextMenu.Closed += (sd, ea) =>
            {
                if (!Uploading) OnClose(sd, ea);
            };

            return _contextMenu;
        }

        private async void OnInsertPath(object sender, RoutedEventArgs e)
        {
            await GuardedAction(() =>
            {
                if (!(DragEventArgs.Data.GetData(DataFormats.FileDrop) is string[] files)) throw new InvalidProgramException("no file selected");
                var relativePath = FileExtensions.MakeRelativePath(TextEditor.Document.FileName, files[0]).Replace('\\', '/');
                return Task.FromResult(_imageService.CreateImageTag(relativePath, Path.GetFileName(files[0])));
            });
        }

        private async void OnUploadToImgur(object sender, RoutedEventArgs e)
        {
            await GuardedAction(async () =>
            {
                Uploading = true;
                var tuple = GetImageData();
                using (tuple.stream)
                {
                    void Progress(object o, UploadProgressChangedEventArgs args) => TextEditor.Dispatcher.InvokeAsync(() =>
                    {
                        if (CancelUpload) ((WebClient)o).CancelAsync();
                        var progressPercentage = (int)(args.BytesSent / (double)args.TotalBytesToSend * 100);
                        ProgressBar.Value = progressPercentage;
                        if (progressPercentage == 100) ProgressBar.IsIndeterminate = true;
                    });

                    void Completed(object o, UploadValuesCompletedEventArgs args)
                    {
                        if (CancelUpload) ((WebClient)o).CancelAsync();
                    }

                    var link = await _imageService.UploadToImgur(tuple.stream, Progress, Completed);

                    return Uri.IsWellFormedUriString(link, UriKind.Absolute)
                        ? _imageService.CreateImageTag(link, tuple.name)
                        : throw new InvalidProgramException(link);
                }
            });
        }

        private async void OnInsertDataUri(object sender, RoutedEventArgs e)
        {
            await GuardedAction(() =>
            {
                (var stream, var imageType, var name) = GetImageData();
                using (stream)
                {
                    Uploading = true;
                    ProgressBar.IsIndeterminate = true;
                    return _imageService.ImageFileToDataUri(stream, imageType, name);
                }
            });
        }

        private async void SaveAs(object sender, RoutedEventArgs e)
        {
            await GuardedAction(async () =>
            {
                var tuple = GetImageData();
                using (tuple.stream)
                {
                    OnClose(sender, e); // Yep, gotta close it or the save dialog wont' work.
                    var fileName = await _imageService.SaveAs(tuple.stream);
                    if (string.IsNullOrWhiteSpace(fileName)) return string.Empty;
                    var relativePath = FileExtensions.MakeRelativePath(TextEditor.Document.FileName, fileName).Replace('\\', '/');
                    return _imageService.CreateImageTag(relativePath, Path.GetFileName(fileName));
                }
            });
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            CloseAction?.Invoke();
        }

        private static MenuItem MenuFactory(string header, PackIconMaterialKind icon, RoutedEventHandler onClick, bool isEnabled = true)
        {
            var menu = new MenuItem
            {
                Header = header,
                StaysOpenOnClick = true,
                Icon = GetMaterialIcon(icon)
            };
            menu.Click += onClick;
            menu.IsEnabled = isEnabled;
            return menu;
        }

        private static PackIconMaterial GetMaterialIcon(PackIconMaterialKind kind)
        {
            return new PackIconMaterial
            {
                Kind = kind,
                Width = 14,
                Height = 14,
                Margin = new Thickness(10, 0, 10, 0)
            };
        }

        private async Task GuardedAction(Func<Task<string>> action)
        {
            try
            {
                _contextMenu.IsOpen = false;
                var text = await action();
                Uploading = false;
                InsertText(TextEditor, DragEventArgs, text);
                TextEditor.TextArea.Focus();
            }
            catch (Exception ex)
            {
                #pragma warning disable 4014
                _notify.Alert(ex.Message); // using await hangs UI here
                #pragma warning restore 4014
            }
            finally
            {
                OnClose(null, null);
            }
        }

        private (Stream stream, string imageType, string name) GetImageData()
        {
            if (UseClipboard)
            {
                var bitmapSource = _imageService.ClipboardDibToBitmapSource();
                var png = _imageService.ToPngArray(bitmapSource);
                return (new MemoryStream(png), "png", "Clipboard");
            }

            if (!(DragEventArgs.Data.GetData(DataFormats.FileDrop) is string[] files)) return (null, null, null);

            var file = files[0];
            var stream = new FileStream(file, FileMode.Open);
            var imageType = Path.GetExtension(file).Trim('.');
            return (stream, imageType, Path.GetFileName(file));
        }

        private static void InsertText(TextEditor textEditor, DragEventArgs dragEventArgs, string text)
        {
            var position = GetInsertOffset(textEditor, dragEventArgs);
            textEditor.Document.Insert(position, text);
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
