using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MarkdownEdit.Controls
{
    public class ImageDropDialogViewModel : INotifyPropertyChanged
    {
        private string _documentFileName;
        private bool _uploading;
        private bool _useClipboardImage;

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