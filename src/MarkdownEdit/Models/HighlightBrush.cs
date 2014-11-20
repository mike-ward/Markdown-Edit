using System;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;

namespace MarkdownEdit
{
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
                _brush = null;
            }
            _brush?.Freeze();
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