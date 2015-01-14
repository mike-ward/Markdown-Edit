using System.Collections.Generic;
using System.IO;
using System.Windows;
using NHunspell;

namespace MarkdownEdit.SpellCheck
{
    public class SpellingService : ISpellingService
    {
        private Hunspell _speller;

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

        public void SetLanguage(string language)
        {
            ClearLanguage();
            var speller = new Hunspell();
            var assemblyFolder = Utility.AssemblyFolder();
            var path = Path.Combine(assemblyFolder, "SpellCheck\\Dictionaries");

            var aff = Path.Combine(path, language + ".aff");
            var dic = Path.Combine(path, language + ".dic");

            if (File.Exists(aff) && File.Exists(dic))
            {
                speller.Load(aff, dic);
                LoadCustomDictonary(speller);
                _speller = speller;
            }
            else
            {
                MessageBox.Show(Application.Current.MainWindow, language + " dictionary not found", App.Title, MessageBoxButton.OK);
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
            File.AppendAllLines(file, new[] {word});
        }
    }
}