using System;
using EditModule.Models;
using EditModule.ViewModels;
using Infrastructure;

namespace EditModule.Features.SyntaxHighlighting
{
    public class SyntaxHighlighting : IEditFeature
    {
        private readonly IAbstractSyntaxTree _abstractSyntaxTree;
        private readonly IBlockBackgroundRenderer _blockBackgroundRenderer;
        private readonly IColorService _colorService;
        private readonly INotify _notify;

        public SyntaxHighlighting(
            IAbstractSyntaxTree abstractSyntaxTree, 
            IBlockBackgroundRenderer blockBackgroundRenderer,
            IColorService colorService,
            INotify notify)
        {
            _abstractSyntaxTree = abstractSyntaxTree;
            _blockBackgroundRenderer = blockBackgroundRenderer;
            _colorService = colorService;
            _notify = notify;
        }

        public void Initialize(EditControlViewModel viewModel)
        {
            var colorizer = new MarkdownHighlightingColorizer(_abstractSyntaxTree);
            viewModel.TextEditor.TextArea.TextView.LineTransformers.Add(colorizer);
            viewModel.TextEditor.TextArea.TextView.BackgroundRenderers.Add(_blockBackgroundRenderer);

            viewModel.TextEditor.TextChanged += (s, e) =>
            {
                try
                {
                    var abs = _abstractSyntaxTree.GenerateAbstractSyntaxTree(viewModel.TextEditor.Text);
                    colorizer.UpdateAbstractSyntaxTree(abs);
                    _blockBackgroundRenderer.UpdateAbstractSyntaxTree(abs);
                    // The block nature of markdown causes edge cases in the syntax hightlighting.
                    // This is the nuclear option but it doesn't seem to cause issues.
                    viewModel.TextEditor.TextArea.TextView.Redraw();
                }
                catch (Exception ex)
                {
                    // See #159
                    _notify.Alert($"Abstract Syntax Tree generation failed: {ex.ToString()}");
                }
            };

            viewModel.ThemeChanged += (s, e) =>
            {
                colorizer.OnThemeChanged(e.Theme);
                _blockBackgroundRenderer.OnThemeChanged(e.Theme);
                viewModel.TextEditor.Foreground = _colorService.CreateBrush(e.Theme.EditorForeground);
                viewModel.TextEditor.Background = _colorService.CreateBrush(e.Theme.EditorBackground);
            };
        }
    }
}
