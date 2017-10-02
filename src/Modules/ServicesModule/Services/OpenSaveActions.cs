using System.IO;
using Infrastructure;
using Microsoft.Win32;

namespace ServicesModule.Services
{
    public class OpenSaveActions : IOpenSaveActions
    {
        public INotify Notify { get; }
        public IStrings Strings { get; }

        public OpenSaveActions(INotify notify, IStrings strings)
        {
            Notify = notify;
            Strings = strings;
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

        public string FromHtml(string file)
        {
            throw new System.NotImplementedException();
        }

        public string FromMicrosoftWord(string file)
        {
            throw new System.NotImplementedException();
        }
    }
}
