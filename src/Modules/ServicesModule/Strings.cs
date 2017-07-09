using Infrastructure;

namespace ServicesModule
{
    public class Strings : IStrings
    {
        public Strings()
        {
            SaveYourChanges = "Save your changes?";
        }

        public string SaveYourChanges { get; }
    }
}
