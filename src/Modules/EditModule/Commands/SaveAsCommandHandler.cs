using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class SaveAsCommandHandler : IEditCommandHandler
    {
        private readonly IOpenSaveActions _openSaveActions;
        private TextEditor _textEditor;

        public SaveAsCommandHandler(IOpenSaveActions openSaveActions)
        {
            _openSaveActions = openSaveActions;
        }

        public string Name { get; } = nameof(SaveAsCommandHandler);

        public void Initialize(EditControlViewModel viewModel)
        {
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            var fileName = _openSaveActions.SaveAsDialog();
            if (string.IsNullOrEmpty(fileName)) return;
            _textEditor.Document.FileName = fileName;
            ApplicationCommands.Save.Execute(null, null);
        }
    }
}
