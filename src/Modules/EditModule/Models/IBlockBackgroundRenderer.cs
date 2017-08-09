using CommonMark.Syntax;
using ICSharpCode.AvalonEdit.Rendering;
using Infrastructure;

namespace EditModule.Models
{
    public interface IBlockBackgroundRenderer : IBackgroundRenderer
    {
        void UpdateAbstractSyntaxTree(Block ast);
        void OnThemeChanged(ITheme theme);
    }

}