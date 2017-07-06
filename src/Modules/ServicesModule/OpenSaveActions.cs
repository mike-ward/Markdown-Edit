using System;
using System.IO;
using Infrastructure;
using Microsoft.Win32;

namespace ServicesModule
{
    public class OpenSaveActions : IOpenSaveActions
    {
        public string Open(Uri file)
        {
            return file != null
                ? File.ReadAllText(file.LocalPath)
                : null;
        }

        public void Save(Uri file, string text)
        {
            File.WriteAllText(file.AbsolutePath, text);
        }

        public Uri OpenDialog()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            return result != null && result.Value
                ? new Uri(dialog.FileName)
                : null;
        }

        public Uri SaveAsDialog()
        {
            var dialog = new SaveFileDialog { OverwritePrompt = true };
            var result = dialog.ShowDialog();
            return result != null && result.Value
                ? new Uri(dialog.FileName)
                : null;
        }
    }
}
