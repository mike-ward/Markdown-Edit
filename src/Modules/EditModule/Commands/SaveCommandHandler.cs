using System;
using System.Windows;
using System.Windows.Input;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Commands
{
    public class SaveCommandHandler : IEditCommandHandler
    {
        private readonly IOpenSaveActions _openSaveActions;
        private readonly INotify _notify;
        private TextEditor _textEditor;

        public SaveCommandHandler(IOpenSaveActions openSaveActions, INotify notify)
        {
            _openSaveActions = openSaveActions;
            _notify = notify;
        }

        public void Initialize(UIElement uiElement, EditControlViewModel viewModel)
        {
            uiElement.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, Execute));
            _textEditor = viewModel.TextEditor;
        }

        public void Execute(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                if (string.IsNullOrEmpty(_textEditor.Document.FileName))
                {
                    ApplicationCommands.SaveAs.Execute(null, null);
                    return;
                }
                _openSaveActions.Save(_textEditor.Document.FileName, _textEditor.Document.Text);
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
