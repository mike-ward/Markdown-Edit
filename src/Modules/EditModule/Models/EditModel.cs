using System;
using System.Windows;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Models
{
    internal class EditModel : IEditModel
    {
        private readonly IOpenSaveActions _openSaveActions;
        private readonly IStrings _strings;
        private readonly INotify _notify;

        public EditModel(IOpenSaveActions openSaveActions, IStrings strings, INotify notify)
        {
            _openSaveActions = openSaveActions;
            _strings = strings;
            _notify = notify;
        }

        public void NewCommandExecuted(TextEditor textEditor)
        {
            if (textEditor.IsModified)
            {
                var result = _notify.ConfirmYesNoCancel(_strings.SaveYourChanges);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes)
                {
                    SaveCommandExecuted(textEditor);
                    if (textEditor.IsModified) return;
                }
            }
            textEditor.Document.Text = string.Empty;
            textEditor.Document.FileName = string.Empty;
            textEditor.IsModified = false;
            textEditor.Focus();
        }

        public void OpenCommandExecuted(TextEditor textEditor, string fileName = null)
        {
            if (textEditor.IsModified)
            {
                var result = _notify.ConfirmYesNoCancel(_strings.SaveYourChanges);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes)
                {
                    SaveCommandExecuted(textEditor);
                    if (textEditor.IsModified) return;
                }
            }

            var file = fileName ?? _openSaveActions.OpenDialog();
            if (string.IsNullOrEmpty(file)) return;

            try
            {
                var text = _openSaveActions.Open(file);
                textEditor.Document.Text = text;
                textEditor.Document.FileName = file;
                textEditor.ScrollToHome();
                textEditor.IsModified = false;
                textEditor.Focus();
            }
            catch (Exception ex)
            {
                _notify.Alert(ex.Message);
            }
        }

        public void SaveCommandExecuted(TextEditor textEditor)
        {
            try
            {
                if (string.IsNullOrEmpty(textEditor.Document.FileName))
                {
                    SaveAsCommandExecuted(textEditor);
                    return;
                }
                _openSaveActions.Save(textEditor.Document.FileName, textEditor.Document.Text);
                textEditor.IsModified = false;
                textEditor.Focus();
            }
            catch (Exception ex)
            {
                _notify.Alert(ex.Message);
            }
        }

        public void SaveAsCommandExecuted(TextEditor textEditor)
        {
            var fileName = _openSaveActions.SaveAsDialog();
            if (string.IsNullOrEmpty(fileName)) return;
            textEditor.Document.FileName = fileName;
            SaveCommandExecuted(textEditor);
        }
    }
}
