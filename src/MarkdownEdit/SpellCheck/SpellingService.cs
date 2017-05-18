using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MarkdownEdit.Models;
using NHunspell;

namespace MarkdownEdit.SpellCheck
{
    public class SpellingService : ISpellingService
    {
        private string _language;
        private Hunspell _speller;

        public bool Spell(string word) { return _speller == null || _speller.Spell(word); }

        public IEnumerable<string> Suggestions(string word) { return _speller.Suggest(word); }

        public void Add(string word)
        {
            if (_speller == null) return;
            _speller.Add(word);
            UpdateCustomDictionary(word);
        }

        public void ClearLanguage() { _speller = null; }

        public string Language { get => _language;
            set => SetLanguage(value);
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

        public string[] Languages()
        {
            return Directory
                .GetFiles(SpellCheckFolder(), "*.dic")
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }

        private static string SpellCheckFolder() => Path.Combine(Utility.AssemblyFolder(), "SpellCheck\\Dictionaries");

        private void SetLanguage(string language)
        {
            try
            {
                ClearLanguage();
                var speller = new Hunspell();
                var path = SpellCheckFolder();

                var aff = Path.Combine(path, language + ".aff");
                var dic = Path.Combine(path, language + ".dic");

                if (File.Exists(aff) && File.Exists(dic))
                {
                    speller.Load(aff, dic);
                    LoadCustomDictonary(speller);
                    _speller = speller;
                    _language = language;
                }
                else
                {
                    Notify.Alert(language + " dictionary not found");
                }
            }
            catch (Exception ex)
            {
                Notify.Alert($"{ex.Message} in {language ?? "null"} file");
            }
        }

        private void LoadCustomDictonary(Hunspell speller)
        {
            try
            {
                var file = CustomDictionaryFile();
                foreach (var word in File.ReadAllLines(file)) speller.Add(word);
            }
            catch (Exception ex)
            {
                Notify.Alert($"{ex.Message} while loading custom dictionary");
            }
        }

        private void UpdateCustomDictionary(string word)
        {
            try
            {
                var file = CustomDictionaryFile();
                File.AppendAllLines(file, new[] {word});
            }
            catch (Exception ex)
            {
                Notify.Alert($"{ex.Message} while updating custom dictionary");
            }
        }
    }
}