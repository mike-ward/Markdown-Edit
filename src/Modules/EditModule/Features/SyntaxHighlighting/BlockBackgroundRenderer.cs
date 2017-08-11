using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using CommonMark.Syntax;
using EditModule.Models;
using ICSharpCode.AvalonEdit.Rendering;
using Infrastructure;

namespace EditModule.Features.SyntaxHighlighting
{
    public class BlockBackgroundRenderer : IBlockBackgroundRenderer
    {
        private IAbstractSyntaxTree AbstractSyntaxTree { get; }
        public IColorService ColorService { get; }

        public BlockBackgroundRenderer(IAbstractSyntaxTree abstractSyntaxTree, IColorService colorService)
        {
            AbstractSyntaxTree = abstractSyntaxTree;
            ColorService = colorService;
        }

        private readonly BlockTag[] _blockTags =
        {
            BlockTag.FencedCode,
            BlockTag.IndentedCode,
            BlockTag.AtxHeading,
            BlockTag.SetextHeading,
            BlockTag.HtmlBlock,
            BlockTag.BlockQuote
        };

        private readonly Dictionary<BlockTag, Brush> _brushes = new Dictionary<BlockTag, Brush>();
        private Block _abstractSyntaxTree;

        public KnownLayer Layer => KnownLayer.Background;

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            var ast = _abstractSyntaxTree;
            if (ast == null) return;

            foreach (var line in textView.VisualLines)
            {
                var rc = BackgroundGeometryBuilder.GetRectsFromVisualSegment(textView, line, 0, 1000).FirstOrDefault();
                if (rc == default(Rect)) continue;

                foreach (var block in AbstractSyntaxTree.EnumerateBlocks(ast.FirstChild))
                {
                    if (block.SourcePosition + block.SourceLength <= line.StartOffset) continue;
                    if (block.SourcePosition > line.LastDocumentLine.EndOffset) break;

                    // Indented code does not terminate (at least in the AST) until a new
                    // block is encountered. Thus blank lines at the end of the block are
                    // highlighted. It looks much better if they're not highlighted.
                    if (block.Tag == BlockTag.IndentedCode)
                    {
                        var length = block.SourcePosition + block.SourceLength - line.StartOffset;
                        if (line.StartOffset + length > textView.Document.TextLength) break;
                        var remainder = textView.Document.GetText(line.StartOffset, length);
                        if (string.IsNullOrWhiteSpace(remainder)) break;
                    }

                    if (_blockTags.Any(tag => tag == block.Tag))
                    {
                        if (_brushes.TryGetValue(block.Tag, out Brush brush) && brush != null)
                        {
                            drawingContext.DrawRectangle(brush, null,
                                new Rect(0, rc.Top, textView.ActualWidth, line.Height));
                        }
                    }
                }
            }
        }

        public void UpdateAbstractSyntaxTree(Block ast) { _abstractSyntaxTree = ast; }

        public void OnThemeChanged(ITheme theme)
        {
            _brushes.Clear();
            var codeBrush = ColorService.CreateBrush(theme.HighlightBlockCode.Background);
            _brushes[BlockTag.FencedCode] = codeBrush;
            _brushes[BlockTag.IndentedCode] = codeBrush;
            _brushes[BlockTag.HtmlBlock] = codeBrush;

            var headingBrush = ColorService.CreateBrush(theme.HighlightHeading.Background);
            _brushes[BlockTag.AtxHeading] = headingBrush;
            _brushes[BlockTag.SetextHeading] = headingBrush;

            _brushes[BlockTag.BlockQuote] = ColorService.CreateBrush(theme.HighlightBlockQuote.Background);
        }
    }
}