using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Infrastructure;
using NHunspell;

namespace ServicesModule.Services
{
    public class SpellCheckService : ISpellCheckService
    {
        private readonly Hunspell _speller;
        private static string SpellCheckFolder() => Path.Combine(Utility.AssemblyFolder(), "Dictionaries");

        public SpellCheckService(INotify notify, ISettings settings)
        {
            try
            {
                _speller = SpellFactory(settings.SpellCheckDictionary);
            }
            catch (Exception ex)
            {
                notify.Alert($"{ex.Message}");
            }
        }

        private static Hunspell SpellFactory(string language)
        {
            var speller = new Hunspell();
            try
            {
                var path = SpellCheckFolder();

                var aff = Path.Combine(path, language + ".aff");
                var dic = Path.Combine(path, language + ".dic");

                speller.Load(aff, dic);
                return speller;

            }
            catch
            {
                speller.Dispose();
                throw;
            }
        }

        public bool Check(string word)
        {
            return _speller?.Spell(word) ?? true;
        }

        public void Add(string word)
        {
            _speller?.Add(word);
        }

        public IEnumerable<string> Suggestions(string word)
        {
            return _speller?.Suggest(word) ?? new List<string>();
        }

        public string[] Languages()
        {
            return Directory
                .GetFiles(SpellCheckFolder(), "*.dic")
                .Select(Path.GetFileNameWithoutExtension)
                .ToArray();
        }
    }
}
