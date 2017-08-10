using EditModule.ViewModels;

namespace EditModule.Features
{
    public class TextEditorOptions : IEditFeature
    {
        public void Initialize(EditControlViewModel viewModel)
        {
            var options = viewModel.TextEditor.Options;
            options.IndentationSize = 2;
            options.AllowToggleOverstrikeMode = true;
            options.EnableHyperlinks = false;
            options.EnableEmailHyperlinks = false;
            options.CutCopyWholeLine = true;
            options.ConvertTabsToSpaces = true;
            options.AllowScrollBelowDocument = true;
            options.EnableRectangularSelection = true;
        }
    }
}
