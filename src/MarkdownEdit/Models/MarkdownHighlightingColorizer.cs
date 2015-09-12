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
        private readonly CommonMarkSettings _commonMarkSettings;
        private Block _abstractSyntaxTree;
        private Theme _theme;

        public MarkdownHighlightingColorizer()
        {
            _commonMarkSettings = CommonMarkSettings.Default.Clone();
            _commonMarkSettings.TrackSourcePosition = true;
        }

        private static readonly Dictionary<BlockTag, Func<Theme, Highlight>> BlockHighlighter = new Dictionary<BlockTag, Func<Theme, Highlight>>
        {
            {BlockTag.AtxHeader, t => t.HighlightHeading},
            {BlockTag.SETextHeader, t => t.HighlightHeading},
            {BlockTag.BlockQuote, t => t.HighlightBlockQuote},
            {BlockTag.ListItem, t => t.HighlightStrongEmphasis},
            {BlockTag.FencedCode, t => t.HighlightBlockCode},
            {BlockTag.IndentedCode, t => t.HighlightBlockCode},
            {BlockTag.HtmlBlock, t => t.HighlightBlockCode},
            {BlockTag.ReferenceDefinition, t => t.HighlightLink}
        };

        private static readonly Dictionary<InlineTag, Func<Theme, Highlight>> InlineHighlighter = new Dictionary<InlineTag, Func<Theme, Highlight>>
        {
            {InlineTag.Code, t => t.HighlightInlineCode},
            {InlineTag.Emphasis, t => t.HighlightEmphasis},
            {InlineTag.Strong, t => t.HighlightStrongEmphasis},
            {InlineTag.Link, t => t.HighlightLink},
            {InlineTag.Image, t => t.HighlightImage},
            {InlineTag.RawHtml, t => t.HighlightBlockCode}
        };

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

        private static IEnumerable<Block> EnumerateSpanningBlocks(Block ast, int startOffset, int endOffset)
        {
            return EnumerateBlocks(ast.FirstChild)
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

            var backgroundBrush = ColorBrush(highlight.Background);
            if (backgroundBrush != null) trp.SetBackgroundBrush(backgroundBrush);

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

        public void OnTextChanged(string text)
        {
            var ast = ParseDocument(text);
            _abstractSyntaxTree = ast;
        }

        public void OnThemeChanged(Theme theme)
        {
            _theme = theme;
        }

        private Block ParseDocument(string text)
        {
            using (var reader = new StringReader(Normalize(text)))
            {
                var ast = CommonMarkConverter.ProcessStage1(reader, _commonMarkSettings);
                CommonMarkConverter.ProcessStage2(ast, _commonMarkSettings);
                return ast;
            }
        }

        private static string Normalize(string value)
        {
            return value.Replace('→', '\t').Replace('␣', ' ');
        }
    }
}