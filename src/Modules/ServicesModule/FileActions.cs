using System;
using System.IO;
using Infrastructure;

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
                return File.ReadAllText(file.ToString());

            }
            catch (Exception ex)
            {
                MessageBox.Alert(ex.Message);
                return null;
            }
        }

        public void Save(Uri file, string text)
        {
            throw new NotImplementedException();
        }

        public Uri OpenDialog()
        {
            throw new NotImplementedException();
        }

        public void SaveAs(string text)
        {
            throw new NotImplementedException();
        }
    }
}
