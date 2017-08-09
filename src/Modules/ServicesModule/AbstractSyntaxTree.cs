using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CommonMark;
using CommonMark.Syntax;
using Infrastructure;

namespace ServicesModule
{
    public class AbstractSyntaxTree : IAbstractSyntaxTree
    {
        private CommonMarkSettings CommonMarkSettings { get; }

        public AbstractSyntaxTree()
        {
            CommonMarkSettings = CommonMarkSettings.Default.Clone();
            CommonMarkSettings.TrackSourcePosition = true;
        }

        public Block GenerateAbstractSyntaxTree(string text)
        {
            using (var reader = new StringReader(Normalize(text)))
            {
                var ast = CommonMarkConverter.ProcessStage1(reader, CommonMarkSettings);
                CommonMarkConverter.ProcessStage2(ast, CommonMarkSettings);
                return ast;
            }
        }

        private static string Normalize(string value) { return value.Replace('→', '\t').Replace('␣', ' '); }

        public Dictionary<BlockTag, Func<ITheme, IHighlight>> BlockHighlighter { get; } = new Dictionary<BlockTag, Func<ITheme, IHighlight>>
        {
            {BlockTag.AtxHeading, t => t.HighlightHeading},
            {BlockTag.SetextHeading, t => t.HighlightHeading},
            {BlockTag.BlockQuote, t => t.HighlightBlockQuote},
            {BlockTag.ListItem, t => t.HighlightStrongEmphasis},
            {BlockTag.FencedCode, t => t.HighlightBlockCode},
            {BlockTag.IndentedCode, t => t.HighlightBlockCode},
            {BlockTag.HtmlBlock, t => t.HighlightBlockCode},
            {BlockTag.ReferenceDefinition, t => t.HighlightLink}
        };

        public Dictionary<InlineTag, Func<ITheme, IHighlight>> InlineHighlighter { get; } = new Dictionary<InlineTag, Func<ITheme, IHighlight>>
        {
            {InlineTag.Code, t => t.HighlightInlineCode},
            {InlineTag.Emphasis, t => t.HighlightEmphasis},
            {InlineTag.Strong, t => t.HighlightStrongEmphasis},
            {InlineTag.Link, t => t.HighlightLink},
            {InlineTag.Image, t => t.HighlightImage},
            {InlineTag.RawHtml, t => t.HighlightBlockCode}
        };

        public IEnumerable<Block> EnumerateSpanningBlocks(Block ast, int startOffset, int endOffset)
        {
            return EnumerateBlocks(ast.FirstChild)
                .Where(b => b.SourcePosition + b.SourceLength > startOffset)
                .TakeWhile(b => b.SourcePosition < endOffset);
        }

        public IEnumerable<Block> EnumerateBlocks(Block block)
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

        public IEnumerable<Inline> EnumerateInlines(Inline inline)
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

        public bool PositionSafeForSmartLink(Block ast, int start, int length)
        {
            if (ast == null) return true;
            var end = start + length;
            var blockTags = new[] { BlockTag.FencedCode, BlockTag.HtmlBlock, BlockTag.IndentedCode, BlockTag.ReferenceDefinition };
            var inlineTags = new[] { InlineTag.Code, InlineTag.Link, InlineTag.RawHtml, InlineTag.Image };
            var lastBlockTag = BlockTag.Document;

            foreach (var block in EnumerateBlocks(ast.FirstChild))
            {
                if (block.SourcePosition + block.SourceLength < start)
                {
                    lastBlockTag = block.Tag;
                    continue;
                }

                if (block.SourcePosition >= end) return !blockTags.Any(tag => tag == lastBlockTag);
                if (blockTags.Any(tag => tag == block.Tag)) return false;

                return !EnumerateInlines(block.InlineContent)
                    .TakeWhile(il => il.SourcePosition < end)
                    .Where(il => il.SourcePosition + il.SourceLength > start)
                    .Any(il => inlineTags.Any(tag => tag == il.Tag));
            }
            return true;
        }
    }
}
