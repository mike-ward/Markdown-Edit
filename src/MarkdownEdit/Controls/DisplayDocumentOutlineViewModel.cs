using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MarkdownEdit.Models;

namespace MarkdownEdit.Controls
{
    internal class DisplayDocumentOutlineViewModel : INotifyPropertyChanged
    {
        private DocumentOutline _outline;

        public DocumentOutline Outline
        {
            get { return _outline; }
            set { Set(ref _outline, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}