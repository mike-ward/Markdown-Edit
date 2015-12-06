using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommonMark.Syntax;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using static MarkdownEdit.Models.AbstractSyntaxTree;

namespace MarkdownEdit.Models
{
    public class MarkdownHighlightingColorizer : DocumentColorizingTransformer
    {
        private Block _abstractSyntaxTree;
        private Theme _theme;

        protected override void ColorizeLine(DocumentLine line)
        {
            var ast = _abstractSyntaxTree;
            if (ast == null) return;

            var theme = _theme;
            if (theme == null) return;

            var start = line.Offset;
            var end = line.EndOffset;
            var leadingSpaces = CurrentContext.GetText(start, end - start).Text.TakeWhile(char.IsWhiteSpace).Count();
            Func<Theme, Highlight> highlighter;

            foreach (var block in EnumerateSpanningBlocks(ast, start, end))
            {
                if (BlockHighlighter.TryGetValue(block.Tag, out highlighter))
                {
                    var magnify = double.NaN;
                    if (block.HeaderLevel == 1) magnify = theme.Header1Height;
                    if (block.HeaderLevel == 2) magnify = theme.Header2Height;

                    var length = (block.Tag == BlockTag.ListItem)
                        ? Math.Min(block.SourceLength, block.ListData.Padding)
                        : block.SourceLength;

                    ApplyLinePart(highlighter(theme), block.SourcePosition, length, start, end, leadingSpaces, magnify);
                }

                foreach (var inline in EnumerateInlines(block.InlineContent)
                    .TakeWhile(il => il.SourcePosition < end)
                    .Where(il => InlineHighlighter.TryGetValue(il.Tag, out highlighter)))
                {
                    var position = inline.SourcePosition;
                    var length = inline.SourceLength;

                    if ((inline.Tag == InlineTag.Link || inline.Tag == InlineTag.Image)
                        && inline.FirstChild?.LiteralContent != null
                        && inline.FirstChild.LiteralContent != inline.TargetUrl)
                    {
                        var literal = inline.FirstChild.LastSibling;
                        var urlPosition = literal.SourcePosition + literal.SourceLength + 1;
                        var urlLength = inline.SourcePosition + inline.SourceLength - urlPosition;
                        if (urlLength > 0) // check for <name@domain.ext> style links
                        {
                            position = urlPosition;
                            length = urlLength;
                        }
                    }

                    // inlines don't magnify
                    ApplyLinePart(highlighter(theme), position, length, start, end, leadingSpaces, double.NaN);
                }
            }
        }

        private void ApplyLinePart(Highlight highlight, int sourceStart, int sourceLength, int lineStart, int lineEnd, int leadingSpaces, double magnify)
        {
            var start = Math.Max(sourceStart, lineStart + leadingSpaces);
            var end = Math.Min(lineEnd, sourceStart + sourceLength);
            if (start < end) ChangeLinePart(start, end, element => ApplyHighlight(element, highlight, magnify));
        }

        private static void ApplyHighlight(VisualLineElement element, Highlight highlight, double magnify)
        {
            var trp = element.TextRunProperties;

            var foregroundBrush = ColorBrush(highlight.Foreground);
            if (foregroundBrush != null) trp.SetForegroundBrush(foregroundBrush);

            // Block background highlighting handled by BlockBackgroundRenderer
            // Otherwise selection highlight does not work as expected
            if (!highlight.Name.Contains("Block"))
            {
                var backgroundBrush = ColorBrush(highlight.Background);
                if (backgroundBrush != null) trp.SetBackgroundBrush(backgroundBrush);
            }

            if (!string.IsNullOrWhiteSpace(highlight.FontWeight) || !string.IsNullOrWhiteSpace(highlight.FontStyle))
            {
                var tf = trp.Typeface;
                var weight = ConvertFontWeight(highlight.FontWeight) ?? tf.Weight;
                var style = ConvertFontStyle(highlight.FontStyle) ?? tf.Style;
                var typeFace = new Typeface(tf.FontFamily, style, weight, tf.Stretch);
                trp.SetTypeface(typeFace);
            }

            if (highlight.Underline) trp.SetTextDecorations(TextDecorations.Underline);
            if (double.IsNaN(magnify) == false) trp.SetFontRenderingEmSize(trp.FontRenderingEmSize*magnify);
        }

        private static Brush ColorBrush(string color)
        {
            if (string.IsNullOrWhiteSpace(color)) return null;
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                var brush = new SolidColorBrush((Color) ColorConverter.ConvertFromString(color));
                brush.Freeze();
                return brush;
            }
            catch (FormatException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        private static FontWeight? ConvertFontWeight(string fontWeight)
        {
            if (string.IsNullOrWhiteSpace(fontWeight)) return null;
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                return (FontWeight) new FontWeightConverter().ConvertFromString(fontWeight);
            }
            catch (FormatException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        private static FontStyle? ConvertFontStyle(string fontStyle)
        {
            if (string.IsNullOrWhiteSpace(fontStyle)) return null;
            try
            {
                // ReSharper disable once PossibleNullReferenceException
                return (FontStyle) (new FontStyleConverter().ConvertFromString(fontStyle));
            }
            catch (FormatException)
            {
                return null;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        public void UpdateAbstractSyntaxTree(Block ast)
        {
            _abstractSyntaxTree = ast;
        }

        public void OnThemeChanged(Theme theme)
        {
            _theme = theme;
        }
    }
}