using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace MarkdownEdit
{
    public class Highlight
    {
        public string Name { get; set; }
        public string Background { get; set; }
        public string Foreground { get; set; }
        public string FontWeight { get; set; }
        public string FontStyle { get; set; }
    }

    public class Theme : INotifyPropertyChanged
    {
        private string _name = "Apple Crisp";
        private string _editorBackground = "#F7F4EF";
        private string _editorForeground = "Black";

        private Highlight _highlightHeading = new Highlight {Name = "Heading", FontWeight = "bold"};
        private Highlight _highlightEmphasis = new Highlight {Name = "Emphasis", FontStyle = "italic"};
        private Highlight _highlightStrongEmphasis = new Highlight {Name = "StrongEmphasis", FontWeight = "bold"};
        private Highlight _highlightInlineCode = new Highlight {Name = "InlineCode", Foreground = "#654", Background = "#eed"};
        private Highlight _highlightBlockCode = new Highlight {Name = "BlockCode", Foreground = "#654"};
        private Highlight _highlightBlockQuote = new Highlight {Name = "BlockQuote", Foreground = "#654"};
        private Highlight _highlightLink = new Highlight {Name = "Link", Foreground = "#654"};
        private Highlight _highlightImage = new Highlight {Name = "Image", Foreground = "#654"};

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }

        public string EditorBackground
        {
            get { return _editorBackground; }
            set
            {
                if (_editorBackground == value) return;
                _editorBackground = value;
                OnPropertyChanged();
            }
        }

        public string EditorForeground
        {
            get { return _editorForeground; }
            set
            {
                if (_editorForeground == value) return;
                _editorForeground = value;
                OnPropertyChanged();
            }
        }

        public Highlight HighlightHeading
        {
            get { return _highlightHeading; }
            set
            {
                if (_highlightHeading == value) return;
                _highlightHeading = value;
                OnPropertyChanged();
            }
        }

        public Highlight HighlightEmphasis
        {
            get { return _highlightEmphasis; }
            set
            {
                if (_highlightEmphasis == value) return;
                _highlightEmphasis = value;
                OnPropertyChanged();
            }
        }

        public Highlight HighlightStrongEmphasis
        {
            get { return _highlightStrongEmphasis; }
            set
            {
                if (_highlightStrongEmphasis == value) return;
                _highlightStrongEmphasis = value;
                OnPropertyChanged();
            }
        }

        public Highlight HighlightInlineCode
        {
            get { return _highlightInlineCode; }
            set
            {
                if (_highlightInlineCode == value) return;
                _highlightInlineCode = value;
                OnPropertyChanged();
            }
        }

        public Highlight HighlightBlockCode
        {
            get { return _highlightBlockCode; }
            set
            {
                if (_highlightBlockCode == value) return;
                _highlightBlockCode = value;
                OnPropertyChanged();
            }
        }

        public Highlight HighlightBlockQuote
        {
            get { return _highlightBlockQuote; }
            set
            {
                if (_highlightBlockQuote == value) return;
                _highlightBlockQuote = value;
                OnPropertyChanged();
            }
        }

        public Highlight HighlightLink
        {
            get { return _highlightLink; }
            set
            {
                if (_highlightLink == value) return;
                _highlightLink = value;
                OnPropertyChanged();
            }
        }

        public Highlight HighlightImage
        {
            get { return _highlightImage; }
            set
            {
                if (_highlightImage == value) return;
                _highlightImage = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal sealed class HighlightBrush : HighlightingBrush
    {
        private readonly SolidColorBrush _brush;

        public HighlightBrush(string color)
        {
            try
            {
                _brush = string.IsNullOrWhiteSpace(color)
                    ? null
                    : (SolidColorBrush)new BrushConverter().ConvertFromString(color);
            }
            catch (FormatException)
            {
                _brush = new SolidColorBrush();
            }
            if (_brush != null) _brush.Freeze();
        }

        public override Brush GetBrush(ITextRunConstructionContext context)
        {
            return _brush;
        }

        public override string ToString()
        {
            return _brush.ToString();
        }
    }
}