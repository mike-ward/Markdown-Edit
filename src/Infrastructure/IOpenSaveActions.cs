namespace Infrastructure
{
    public interface IOpenSaveActions
    {
        string Open(string file);
        void Save(string file, string text);
        string OpenDialog();
        string SaveAsDialog();
        string FromHtml(string file);
        string FromMicrosoftWord(string file);
    }
}