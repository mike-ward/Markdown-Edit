using System;
using System.IO;

namespace Infrastructure
{
    public interface IFileActions
    {
        string Open(Uri file);
        void Save(Uri file, string text);
        Uri OpenDialog();
        void SaveAs(string text);
    }
}