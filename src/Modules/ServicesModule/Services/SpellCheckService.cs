using System.Collections.Generic;
using Infrastructure;

namespace ServicesModule.Services
{
    public class SpellCheckService : ISpellCheckService
    {
        public IEnumerable<string> Suggestions(string word)
        {
            return new string[0];
        }

        public bool Spell(string word)
        {
            return true;
        }
    }
}
