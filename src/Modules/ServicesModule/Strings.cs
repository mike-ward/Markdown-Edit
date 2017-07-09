using Infrastructure;

namespace ServicesModule
{
    public class Strings : IStrings
    {
        public Strings()
        {
            NewDocumentName = "New Document";
            SaveYourChanges = "Save your changes?";
        }

        public string NewDocumentName { get; }
        public string SaveYourChanges { get; }
    }
}
