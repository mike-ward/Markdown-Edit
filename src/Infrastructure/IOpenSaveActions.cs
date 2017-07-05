using System;
using System.IO;

namespace Infrastructure
{
    public interface IOpenSaveActions
    {
        string Open(Uri file);
        void Save(Uri file, string text);
        Uri OpenDialog();
        Uri SaveAsDialog();
    }
}