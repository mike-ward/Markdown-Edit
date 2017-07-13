using System;
using System.Windows;
using ICSharpCode.AvalonEdit;
using Infrastructure;

namespace EditModule.Models
{
    internal class EditModel : IEditModel
    {
        public IOpenSaveActions OpenSaveActions { get; }
        public IStrings Strings { get; }
        public INotify Notify { get; }

        public EditModel(IOpenSaveActions openSaveActions, IStrings strings, INotify notify)
        {
            OpenSaveActions = openSaveActions;
            Strings = strings;
            Notify = notify;
        }

        public void NewCommandHandler(TextEditor textEditor)
        {
            if (textEditor.IsModified)
            {
                var result = Notify.ConfirmYesNoCancel(Strings.SaveYourChanges);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes)
                {
                    SaveCommandHandler(textEditor);
                    if (textEditor.IsModified) return;
                }
            }
            textEditor.Document.Text = string.Empty;
            textEditor.Document.FileName = string.Empty;
            textEditor.IsModified = false;
            textEditor.Focus();
        }

        public void OpenCommandHandler(TextEditor textEditor, string fileName = null)
        {
            if (textEditor.IsModified)
            {
                var result = Notify.ConfirmYesNoCancel(Strings.SaveYourChanges);
                if (result == MessageBoxResult.Cancel) return;
                if (result == MessageBoxResult.Yes)
                {
                    SaveCommandHandler(textEditor);
                    if (textEditor.IsModified) return;
                }
            }

            var file = fileName ?? OpenSaveActions.OpenDialog();
            if (string.IsNullOrEmpty(file)) return;

            try
            {
                var text = OpenSaveActions.Open(file);
                textEditor.Document.Text = text;
                textEditor.Document.FileName = file;
                textEditor.ScrollToHome();
                textEditor.IsModified = false;
                textEditor.Focus();
            }
            catch (Exception ex)
            {
                Notify.Alert(ex.Message);
            }
        }

        public void SaveCommandHandler(TextEditor textEditor)
        {
            try
            {
                if (string.IsNullOrEmpty(textEditor.Document.FileName))
                {
                    SaveAsCommandHandler(textEditor);
                    return;
                }
                OpenSaveActions.Save(textEditor.Document.FileName, textEditor.Document.Text);
                textEditor.IsModified = false;
                textEditor.Focus();
            }
            catch (Exception ex)
            {
                Notify.Alert(ex.Message);
            }
        }

        public void SaveAsCommandHandler(TextEditor textEditor)
        {
            var fileName = OpenSaveActions.SaveAsDialog();
            if (string.IsNullOrEmpty(fileName)) return;
            textEditor.Document.FileName = fileName;
            SaveCommandHandler(textEditor);
        }
    }
}
