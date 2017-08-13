using System;
using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class OpenCommandHandler : IEditCommandHandler
    {
        private readonly IOpenSaveActions _openSaveActions;
        private readonly INotify _notify;
        private readonly IStrings _strings;
        private TextEditor _textEditor;

        public OpenCommandHandler(IOpenSaveActions openSaveActions, INotify notify, IStrings strings)
        {
            _openSaveActions = openSaveActions;
            _notify = notify;
            _strings = strings;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open, Execute));
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

            var file = ea.Parameter as string ?? _openSaveActions.OpenDialog();
            if (string.IsNullOrEmpty(file)) return;

            try
            {
                var text = _openSaveActions.Open(file);
                _textEditor.Document.Text = text;
                _textEditor.Document.FileName = file;
                _textEditor.ScrollToHome();
                _textEditor.IsModified = false;
                _textEditor.Focus();
            }
            catch (Exception ex)
            {
                _notify.Alert(ex.Message);
            }
        }
    }
}
