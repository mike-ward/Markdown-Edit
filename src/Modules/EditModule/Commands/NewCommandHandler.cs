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

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.New, Execute));
            _textEditor = viewModel.TextEditor;
        }

        public async void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            if (_textEditor.IsModified)
            {
                var result = await _notify.ConfirmYesNoCancel(_strings.SaveYourChanges);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes)
                {
                    ApplicationCommands.Save.Execute(null, (Window)sender);
                    if (_textEditor.IsModified) return;
                }
            }
            SetToNew();
        }

        private void SetToNew()
        {
            _textEditor.Document.Text = string.Empty;
            _textEditor.Document.FileName = string.Empty;
            _textEditor.IsModified = false;
            _textEditor.Focus();
        }
    }
}
