using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class NewCommandHandler : IEditCommandHandler
    {
        private TextEditor _textEditor;
        private readonly IStrings _strings;
        private readonly INotify _notify;

        public NewCommandHandler(IStrings strings, INotify notify)
        {
            _strings = strings;
            _notify = notify;
        }

        public string Name { get; } = nameof(NewCommandHandler);

        public void Initialize(EditControlViewModel viewModel)
        {
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            if (_textEditor.IsModified)
            {
                var result = _notify.ConfirmYesNoCancel(_strings.SaveYourChanges);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes)
                {
                    ApplicationCommands.Save.Execute(null, null);
                    if (_textEditor.IsModified) return;
                }
            }
            _textEditor.Document.Text = string.Empty;
            _textEditor.Document.FileName = string.Empty;
            _textEditor.IsModified = false;
            _textEditor.Focus();
        }
    }
}
