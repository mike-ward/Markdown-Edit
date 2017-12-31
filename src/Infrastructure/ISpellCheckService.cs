using System.Collections.Generic;

namespace Infrastructure
{
    public interface ISpellCheckService
    {
        bool Spell(string word);
        IEnumerable<string> Suggestions(string word);
    }
}