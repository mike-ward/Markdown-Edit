using System;
using System.IO;
using Infrastructure;
using Microsoft.Win32;

namespace ServicesModule
{
    public class FileActions : IFileActions
    {
        public IMessageBox MessageBox { get; }

        public FileActions(IMessageBox messageBox)
        {
            MessageBox = messageBox;
        }

        public string Open(Uri file)
        {
            try
            {
                return file != null 
                    ? File.ReadAllText(file.AbsolutePath) 
                    : string.Empty;

            }
            catch (Exception ex)
            {
                MessageBox.Alert(ex.Message);
                return string.Empty;
            }
        }

        public void Save(Uri file, string text)
        {
            throw new NotImplementedException();
        }

        public Uri OpenDialog()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            return result != null && result.Value 
                ? new Uri(dialog.FileName) 
                : null;
        }

        public void SaveAs(string text)
        {
            throw new NotImplementedException();
        }
    }
}
