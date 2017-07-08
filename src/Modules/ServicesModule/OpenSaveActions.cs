using System.IO;
using System.Windows;
using Infrastructure;
using Microsoft.Win32;

namespace ServicesModule
{
    public class OpenSaveActions : IOpenSaveActions
    {
        public INotify Notify { get; }

        public OpenSaveActions(INotify notify)
        {
            Notify = notify;
        }

        public string Open(string file)
        {
            return File.ReadAllText(file);
        }

        public void Save(string file, string text)
        {
            File.WriteAllText(file, text);
        }

        public string OpenDialog()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            return result != null && result.Value
                ? dialog.FileName
                : null;
        }

        public string SaveAsDialog()
        {
            var dialog = new SaveFileDialog { OverwritePrompt = true };
            var result = dialog.ShowDialog();
            return result != null && result.Value
                ? dialog.FileName
                : null;
        }

        public MessageBoxResult PromptToSave(string file, string text)
        {
            var result = Notify.ConfirmYesNoCancel("Save your changes");
            if (result == MessageBoxResult.Yes) Save(file, text);
            return result;
        }
    }
}
