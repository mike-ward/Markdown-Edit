using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NHunspell;

namespace MarkdownEdit.SpellCheck
{
    public class SpellingService : ISpellingService
    {
        private static readonly Dictionary<SpellingLanguages, string> LangLookup;
        private Hunspell _speller;

        static SpellingService()
        {
            LangLookup = new Dictionary<SpellingLanguages, string>
            {
                {SpellingLanguages.Australian, "en_AU"},
                {SpellingLanguages.Canadian, "en_CA"},
                {SpellingLanguages.UnitedStates, "en_US"},
                {SpellingLanguages.UnitedKingdom, "en_GB"},
                {SpellingLanguages.Spain, "es_ES"},
                {SpellingLanguages.Germany, "de_DE"}
            };
        }

        public bool Spell(string word)
        {
            return _speller == null || _speller.Spell(word);
        }

        public IEnumerable<string> Suggestions(string word)
        {
            return _speller.Suggest(word);
        }

        public void ClearLanguages()
        {
            _speller = null;
        }

        public void SetLanguage(SpellingLanguages language)
        {
            _speller = new Hunspell();
            var languageKey = LangLookup[language];
            var assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase.Substring(8).Replace('/', '\\'));
            var path = Path.Combine(assemblyFolder, "SpellCheck\\Dictionaries");

            var aff = Path.Combine(path, languageKey + ".aff");
            var dic = Path.Combine(path, languageKey + ".dic");

            if (File.Exists(aff) && File.Exists(dic))
            {
                _speller.Load(aff, dic);
            }
            else
            {
                Debug.WriteLine("dictionary not found");
            }
        }
    }
}