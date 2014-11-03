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

        public void Add(string word)
        {
            if (_speller == null) return; 
            _speller.Add(word);
            UpdateCustomDictionary(word);
        }

        public void ClearLanguage()
        {
            _speller = null;
        }

        public void SetLanguage(SpellingLanguages language)
        {
            ClearLanguage();
            var speller = new Hunspell();
            var languageKey = LangLookup[language];
            var assemblyFolder = Utility.AssemblyFolder();
            var path = Path.Combine(assemblyFolder, "SpellCheck\\Dictionaries");

            var aff = Path.Combine(path, languageKey + ".aff");
            var dic = Path.Combine(path, languageKey + ".dic");

            if (File.Exists(aff) && File.Exists(dic))
            {
                speller.Load(aff, dic);
                LoadCustomDictonary(speller);
                _speller = speller;
            }
            else
            {
                Debug.WriteLine("dictionary not found");
            }
        }

        private void LoadCustomDictonary(Hunspell speller)
        {
            var file = CustomDictionaryFile();
            foreach (var word in File.ReadAllLines(file)) speller.Add(word);
        }

        public string CustomDictionaryFile()
        {
            var file = Path.Combine(UserSettings.SettingsFolder, "custom_dictionary.txt");
            if (File.Exists(file) == false)
            {
                Directory.CreateDirectory(UserSettings.SettingsFolder);
                File.WriteAllText(file, string.Empty);
            }
            return file;
        }

        private void UpdateCustomDictionary(string word)
        {
            var file = CustomDictionaryFile();
            File.AppendAllLines(file, new [] { word });
        }
    }
}