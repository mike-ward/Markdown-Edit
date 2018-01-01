using System.Collections.Generic;

namespace Infrastructure
{
    public interface ISpellCheckService
    {
        bool Check(string word);
        IEnumerable<string> Suggestions(string word);
        string[] Languages();
    }
}