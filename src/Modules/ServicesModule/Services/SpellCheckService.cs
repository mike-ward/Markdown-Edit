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

        public SpellCheckService(INotify notify)
        {
            try
            {
                _speller = CreateSpeller(CultureInfo.CurrentUICulture.Name);
            }
            catch
            {
                try
                {
                    _speller = CreateSpeller("en_US");
                }
                catch (Exception ex)
                {
                    notify.Alert($"{ex.Message}");
                }
            }
        }

        private static Hunspell CreateSpeller(string language)
        {
            var speller = new Hunspell();
            var path = SpellCheckFolder();

            var aff = Path.Combine(path, language + ".aff");
            var dic = Path.Combine(path, language + ".dic");

            speller.Load(aff, dic);
            return speller;
        }

        public bool Check(string word)
        {
            return _speller?.Spell(word) ?? true;
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
