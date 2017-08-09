using System;
using System.Collections.Generic;
using CommonMark.Syntax;

namespace Infrastructure
{
    public interface IAbstractSyntaxTree
    {
        Dictionary<BlockTag, Func<ITheme, IHighlight>> BlockHighlighter { get; }
        Dictionary<InlineTag, Func<ITheme, IHighlight>> InlineHighlighter { get; }
        Block GenerateAbstractSyntaxTree(string text);
        IEnumerable<Block> EnumerateSpanningBlocks(Block ast, int startOffset, int endOffset);
        IEnumerable<Block> EnumerateBlocks(Block block);
        IEnumerable<Inline> EnumerateInlines(Inline inline);
        bool PositionSafeForSmartLink(Block ast, int start, int length);
    }
}