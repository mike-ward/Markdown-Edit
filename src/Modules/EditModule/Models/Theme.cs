using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Infrastructure;
using ServicesModule;

namespace EditModule.Models
{
   
    public class Theme : INotifyPropertyChanged, ITheme
    {
        private string _name = "Zenburn";
        private string _editorBackground = "#404040";
        private string _editorForeground = "#ccc";
        private string _spellCheckError = "#f00";
        private double _header1Height = 1.3;
        private double _header2Height = 1.2;

        private IHighlight _highlightHeading = new Highlight { Name = "Heading", FontWeight = "bold", Foreground = "#6c71c4" };
        private IHighlight _highlightEmphasis = new Highlight { Name = "Emphasis", FontStyle = "italic", Foreground = "#cb4b16" };
        private IHighlight _highlightStrongEmphasis = new Highlight { Name = "StrongEmphasis", FontWeight = "bold", Foreground = "orange" };
        private IHighlight _highlightInlineCode = new Highlight { Name = "InlineCode", Foreground = "#7F9F7F" };
        private IHighlight _highlightBlockCode = new Highlight { Name = "BlockCode", Foreground = "#7F9F7F" };
        private IHighlight _highlightBlockQuote = new Highlight { Name = "BlockQuote", Foreground = "#8ACCCF" };
        private IHighlight _highlightLink = new Highlight { Name = "Link", Foreground = "#2aa198", Underline = true };
        private IHighlight _highlightImage = new Highlight { Name = "Image", Foreground = "#6F8F3F", FontWeight = "bold" };

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        public string EditorBackground
        {
            get => _editorBackground;
            set => Set(ref _editorBackground, value);
        }

        public string EditorForeground
        {
            get => _editorForeground;
            set => Set(ref _editorForeground, value);
        }

        public double Header1Height
        {
            get => _header1Height;
            set => Set(ref _header1Height, value);
        }

        public double Header2Height
        {
            get => _header2Height;
            set => Set(ref _header2Height, value);
        }

        public IHighlight HighlightHeading
        {
            get => _highlightHeading;
            set => Set(ref _highlightHeading, value);
        }

        public IHighlight HighlightEmphasis
        {
            get => _highlightEmphasis;
            set => Set(ref _highlightEmphasis, value);
        }

        public IHighlight HighlightStrongEmphasis
        {
            get => _highlightStrongEmphasis;
            set => Set(ref _highlightStrongEmphasis, value);
        }

        public IHighlight HighlightInlineCode
        {
            get => _highlightInlineCode;
            set => Set(ref _highlightInlineCode, value);
        }

        public IHighlight HighlightBlockCode
        {
            get => _highlightBlockCode;
            set => Set(ref _highlightBlockCode, value);
        }

        public IHighlight HighlightBlockQuote
        {
            get => _highlightBlockQuote;
            set => Set(ref _highlightBlockQuote, value);
        }

        public IHighlight HighlightLink
        {
            get => _highlightLink;
            set => Set(ref _highlightLink, value);
        }

        public IHighlight HighlightImage
        {
            get => _highlightImage;
            set => Set(ref _highlightImage, value);
        }

        public string SpellCheckError
        {
            get => _spellCheckError;
            set => Set(ref _spellCheckError, value);
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value) == false)
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
