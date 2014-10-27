using System.Collections.Generic;

namespace MarkdownEdit.SpellCheck
{
    public interface ISpellingService
    {
        void ClearLanguage();
        void SetLanguage(SpellingLanguages language);
        bool Spell(string word);
        IEnumerable<string> Suggestions(string word);
        void Add(string word);
    }
}