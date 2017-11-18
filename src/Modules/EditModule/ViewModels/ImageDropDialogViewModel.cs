using System;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using MahApps.Metro.IconPacks;
using Prism.Mvvm;
using ServicesModule.Services;

namespace EditModule.ViewModels
{
    public class ImageDropDialogViewModel : BindableBase
    {
        private readonly ImageService _imageService;
        private readonly INotify _notify;
        private bool _uploading;

        public TextEditor TextEditor { get; set; }
        public DragEventArgs DragEventArgs { get; set; }
        public bool UseClipboardImage { get; set; }
        public Action CloseAction { get; set; }

        public ImageDropDialogViewModel(ImageService imageService, INotify notify)
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
            var contextMenu = new ContextMenu { IsOpen = true, StaysOpen = true };
            var items = contextMenu.Items;

            // todo: localize
            items.Add(MenuFactory("Insert Path", PackIconMaterialKind.Image, OnClose));
            items.Add(MenuFactory("Upload to Imgur", PackIconMaterialKind.CloudUpload, OnClose));
            items.Add(MenuFactory("As Data URI", PackIconMaterialKind.Xml, OnInsertDataUri));
            items.Add(MenuFactory("Save As", PackIconMaterialKind.FileDocument, OnClose));
            items.Add(new Separator());
            items.Add(MenuFactory("Cancel", PackIconMaterialKind.Close, OnClose));

            return contextMenu;
        }

        private void OnInsertDataUri(object sender, RoutedEventArgs e)
        {
            GuardedAction(() => _imageService.ImageFileToDataUri(DroppedFiles()[0]));
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            CloseAction?.Invoke();
        }

        private static MenuItem MenuFactory(string header, PackIconMaterialKind icon, RoutedEventHandler onClick)
        {
            var menu = new MenuItem
            {
                Header = header,
                StaysOpenOnClick = true,
                Icon = GetMaterialIcon(icon)
            };
            menu.Click += onClick;
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

        private void GuardedAction(Func<string> action)
        {
            try
            {
                var text = action();
                InsertText(TextEditor, DragEventArgs, text);
                OnClose(null, null);
            }
            catch (Exception ex)
            {
                _notify.Alert(ex.Message);
            }
            finally
            {
                OnClose(null, null);
            }
        }

        private static string CreateImageTag(string link, string title)
        {
            return $"![{title}]({link})\n";
        }

        private string[] DroppedFiles()
        {
            var files = DragEventArgs.Data.GetData(DataFormats.FileDrop) as string[];
            return files;
        }

        private static void InsertText(TextEditor textEditor, DragEventArgs dragEventArgs, string tag)
        {
            textEditor.Document.Insert(GetInsertOffset(textEditor, dragEventArgs), tag);
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
