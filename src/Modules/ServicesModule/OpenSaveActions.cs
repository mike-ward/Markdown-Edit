using System;
using System.IO;
using Infrastructure;
using Microsoft.Win32;

namespace ServicesModule
{
    public class OpenSaveActions : IOpenSaveActions
    {
        public IMessageBox MessageBox { get; }

        public OpenSaveActions(IMessageBox messageBox)
        {
            MessageBox = messageBox;
        }

        public string Open(Uri file)
        {
            try
            {
                return file != null 
                    ? File.ReadAllText(file.AbsolutePath) 
                    : null;

            }
            catch (Exception ex)
            {
                MessageBox.Alert(ex.Message);
                return null;
            }
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
            var dialog = new SaveFileDialog {OverwritePrompt = true};
            var result = dialog.ShowDialog();
            return result != null && result.Value
                ? new Uri(dialog.FileName)
                : null;
        }
    }
}
