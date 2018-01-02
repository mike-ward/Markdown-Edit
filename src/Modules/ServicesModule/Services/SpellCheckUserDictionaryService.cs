using System;
using System.IO;
using System.Linq;
using Infrastructure;

namespace ServicesModule.Services
{
    public class SpellCheckUserDictionaryService : ISpellCheckUserDictionaryService
    {
        private readonly ISpellCheckService _spellCheckService;
        private readonly INotify _notify;

        public SpellCheckUserDictionaryService(ISpellCheckService spellCheckService, INotify notify)
        {
            _spellCheckService = spellCheckService;
            _notify = notify;
            Load();
        }

        private static string UserDictionary()
        {
            var dictionary = Path.Combine(Globals.UserSettingsFolder, "user-dictionary.txt");
            if (!File.Exists(dictionary)) File.WriteAllText(dictionary, string.Empty);
            return dictionary;
        }

        public void Load()
        {
            try
            {
                var dictionary = UserDictionary();
                var words = File.ReadAllLines(dictionary);
                foreach (var word in words.Where(string.IsNullOrWhiteSpace)) _spellCheckService.Add(word);
            }
            catch (Exception ex)
            {
                _notify.Alert($"{ex.Message} while loading user dictionary");
            }
        }

        public void AddWord(string word)
        {
            try
            {
                _spellCheckService.Add(word);
                var dictionary = UserDictionary();
                File.AppendAllLines(dictionary, new []{ word });
            }
            catch (Exception ex)
            {
                _notify.Alert($"{ex.Message} while updating custom dictionary");
            }
        }
    }
}
