using System.Windows.Controls;
using Prism.Mvvm;

namespace EditModule.ViewModels
{
    public class ImageDropDialogViewModel : BindableBase
    {
        private string _documentFileName;
        private bool _uploading;
        private bool _useClipboardImage;

        public string DocumentFileName
        {
            get => _documentFileName;
            set => SetProperty(ref _documentFileName, value);
        }

        public bool Uploading
        {
            get => _uploading;
            set => SetProperty(ref _uploading, value);
        }

        public bool UseClipboardImage
        {
            get => _useClipboardImage;
            set => SetProperty(ref _useClipboardImage, value);
        }

        public ContextMenu CreateContextMenu()
        {
            return new ContextMenu();
        }
    }
}
