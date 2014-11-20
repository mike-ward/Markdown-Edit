using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MarkdownEdit
{
    public class Theme : INotifyPropertyChanged
    {
        private string _name = "Milk Toast";
        private string _editorBackground = "#F9F6F1";
        private string _editorForeground = "#333";

        private Highlight _highlightHeading = new Highlight {Name = "Heading", FontWeight = "bold"};
        private Highlight _highlightEmphasis = new Highlight {Name = "Emphasis", FontStyle = "italic"};
        private Highlight _highlightStrongEmphasis = new Highlight {Name = "StrongEmphasis", FontWeight = "bold"};
        private Highlight _highlightInlineCode = new Highlight {Name = "InlineCode", Foreground = "#333", Background = "#ebeff3"};
        private Highlight _highlightBlockCode = new Highlight {Name = "BlockCode", Foreground = "#654"};
        private Highlight _highlightBlockQuote = new Highlight {Name = "BlockQuote", Foreground = "#654"};
        private Highlight _highlightLink = new Highlight {Name = "Link", Foreground = "#654"};
        private Highlight _highlightImage = new Highlight {Name = "Image", Foreground = "#654"};

        public string Name
        {
            get { return _name; }
            set { Set(ref _name, value); }
        }

        public string EditorBackground
        {
            get { return _editorBackground; }
            set { Set(ref _editorBackground, value); }
        }

        public string EditorForeground
        {
            get { return _editorForeground; }
            set { Set(ref _editorForeground, value); }
        }

        public Highlight HighlightHeading
        {
            get { return _highlightHeading; }
            set { Set(ref _highlightHeading, value); }
        }

        public Highlight HighlightEmphasis
        {
            get { return _highlightEmphasis; }
            set { Set(ref _highlightEmphasis, value); }
        }

        public Highlight HighlightStrongEmphasis
        {
            get { return _highlightStrongEmphasis; }
            set { Set(ref _highlightStrongEmphasis, value); }
        }

        public Highlight HighlightInlineCode
        {
            get { return _highlightInlineCode; }
            set { Set(ref _highlightInlineCode, value); }
        }

        public Highlight HighlightBlockCode
        {
            get { return _highlightBlockCode; }
            set { Set(ref _highlightBlockCode, value); }
        }

        public Highlight HighlightBlockQuote
        {
            get { return _highlightBlockQuote; }
            set { Set(ref _highlightBlockQuote, value); }
        }

        public Highlight HighlightLink
        {
            get { return _highlightLink; }
            set { Set(ref _highlightLink, value); }
        }

        public Highlight HighlightImage
        {
            get { return _highlightImage; }
            set { Set(ref _highlightImage, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            OnPropertyChanged(propertyName);
        }
    }
}