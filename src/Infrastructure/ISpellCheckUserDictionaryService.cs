namespace Infrastructure
{
    public interface ISpellCheckUserDictionaryService
    {
        void Load();
        void AddWord(string word);
    }
}