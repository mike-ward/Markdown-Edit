using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommonMark;
using CommonMark.Syntax;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace MarkdownEdit.Models
{
    public class MarkdownHighlightingColorizer : DocumentColorizingTransformer
    {
        private Block _abstractSyntaxTree;
        private Theme _theme;

        private static readonly Dictionary<InlineTag, Func<Theme, Highlight>> InlineHighlight = new Dictionary<InlineTag, Func<Theme, Highlight>>
        {
            {InlineTag.Code, t => t.HighlightInlineCode},
            {InlineTag.Emphasis, t => t.HighlightEmphasis},
            {InlineTag.Strong, t => t.HighlightStrongEmphasis},
            {InlineTag.Link, t => t.HighlightLink},
            {InlineTag.Image, t => t.HighlightImage}
        };

        protected override void ColorizeLine(DocumentLine line)
        {
            var ast = _abstractSyntaxTree;
            if (ast == null) return;

            var theme = _theme;
            if (theme == null) return;

            var start = line.Offset;
            var end = line.EndOffset;

            foreach (var block in EnumerateSpanningBlocks(ast, start, end))
            {
                switch (block.Tag)
                {
                    case BlockTag.AtxHeader:
                    case BlockTag.SETextHeader:
                        var magnify = block.HeaderLevel == 1 ? theme.Header1Height : block.HeaderLevel == 2 ? theme.Header2Height : 1.0;
                        ApplyLinePart(theme.HighlightHeading, block.SourcePosition, block.SourceLength, start, end, magnify);
                        break;

                    case BlockTag.BlockQuote:
                        ApplyLinePart(theme.HighlightBlockQuote, block.SourcePosition, block.SourceLength, start, end, 1.0);
                        break;

                    case BlockTag.ListItem:
                        var length = Math.Min(block.SourceLength, block.ListData.Padding);
                        ApplyLinePart(theme.HighlightStrongEmphasis, block.SourcePosition, length, start, end, 1.0);
                        break;

                    case BlockTag.FencedCode:
                    case BlockTag.IndentedCode:
                        ApplyLinePart(theme.HighlightBlockCode, block.SourcePosition, block.SourceLength, start, end, 1.0);
                        break;
                }

                foreach (var inline in EnumerateInlines(block.InlineContent).TakeWhile(il => il.SourcePosition < end))
                {
                    Func<Theme, Highlight> highlight;
                    if (InlineHighlight.TryGetValue(inline.Tag, out highlight))
                    {
                        ApplyLinePart(highlight(theme), inline.SourcePosition, inline.SourceLength, start, end, 1.0);
                    }
                }
            }
        }

        private void ApplyLinePart(Highlight highlight, int sourceStart, int sourceLength, int lineStart, int lineEnd, double magnify)
        {
            var text = CurrentContext.GetText(lineStart, lineEnd - lineStart);
            var leadingSpaces = text.Text.TakeWhile(char.IsWhiteSpace).Count();
            var start = Math.Max(sourceStart, lineStart + leadingSpaces);
            var end = Math.Min(lineEnd, sourceStart + sourceLength);
            if (start < end) ChangeLinePart(start, end, element => ApplyHighlight(element, highlight, magnify));
        }

        private static IEnumerable<Block> EnumerateSpanningBlocks(Block ast, int startOffset, int endOffset)
        {
            return EnumerateBlocks(ast.FirstChild)
                .Where(b => b.SourcePosition < endOffset)
                .Where(b => (b.SourcePosition + b.SourceLength) > startOffset)
                .TakeWhile(b => b.SourcePosition < endOffset);
        }

        private static IEnumerable<Block> EnumerateBlocks(Block block)
        {
            if (block == null) yield break;
            var stack = new Stack<Block>();
            stack.Push(block);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                if (next.NextSibling != null) stack.Push(next.NextSibling);
                if (next.FirstChild != null) stack.Push(next.FirstChild);
            }
        }

        private static IEnumerable<Inline> EnumerateInlines(Inline inline)
        {
            if (inline == null) yield break;
            var stack = new Stack<Inline>();
            stack.Push(inline);
            while (stack.Any())
            {
                var next = stack.Pop();
                yield return next;
                if (next.NextSibling != null) stack.Push(next.NextSibling);
                if (next.FirstChild != null) stack.Push(next.FirstChild);
            }
        }

        private static void ApplyHighlight(VisualLineElement element, Highlight highlight, double magnify)
        {
            var trp = element.TextRunProperties;

            var foregroundBrush = ColorBrush(highlight.Foreground);
            if (foregroundBrush != null) trp.SetForegroundBrush(foregroundBrush);

            var backgroundBrush = ColorBrush(highlight.Background);
            if (backgroundBrush != null) trp.SetBackgroundBrush(backgroundBrush);

            if (!string.IsNullOrWhiteSpace(highlight.FontWeight) || !string.IsNullOrWhiteSpace(highlight.FontStyle))
            {
                var tf = element.TextRunProperties.Typeface;
                var weight = ConvertFontWeight(highlight.FontWeight) ?? tf.Weight;
                var style = ConvertFontStyle(highlight.FontStyle) ?? tf.Style;
                var typeFace = new Typeface(tf.FontFamily, style, weight, tf.Stretch);
                trp.SetTypeface(typeFace);
            }

            if (highlight.Underline) trp.SetTextDecorations(TextDecorations.Underline);
            if (Math.Abs(magnify - 1.0) > .000001) trp.SetFontRenderingEmSize(trp.FontRenderingEmSize * magnify);
        }

        private static Brush ColorBrush(string color)
        {
            if (string.IsNullOrWhiteSpace(color)) return null;
            try
            {
                return new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
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
                return (FontWeight)new FontWeightConverter().ConvertFromString(fontWeight);
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
                return (FontStyle)(new FontStyleConverter().ConvertFromString(fontStyle));
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

        public void OnTextChanged(string text)
        {
            var ast = ParseDocument(text);
            _abstractSyntaxTree = ast;
        }

        public void OnThemeChanged(Theme theme)
        {
            _theme = theme;
        }

        private static Block ParseDocument(string text)
        {
            using (var reader = new StringReader(Normalize(text)))
            {
                var settings = new CommonMarkSettings {TrackSourcePosition = true};
                var ast = CommonMarkConverter.ProcessStage1(reader, settings);
                CommonMarkConverter.ProcessStage2(ast, settings);
                return ast;
            }
        }

        private static string Normalize(string value)
        {
            value = value.Replace('→', '\t');
            value = value.Replace('␣', ' ');
            return value;
        }
    }
}