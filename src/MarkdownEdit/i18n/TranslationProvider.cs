using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using MarkdownEdit.Models;

namespace MarkdownEdit.i18n
{
    internal static class TranslationProvider
    {
        private static readonly Language _language;

        static TranslationProvider()
        {
            _language = Load(Thread.CurrentThread.CurrentUICulture);
        }

        private static Language Load(CultureInfo cultureInfo)
        {
            try
            {
                var name = cultureInfo.TwoLetterISOLanguageName;
                var path = Path.Combine(Utility.AssemblyFolder(), "Languages", name);
                var file = path + "\\local.txt";
                var text = File.ReadAllText(file, Encoding.UTF8);
                return Parse(name, text);
            }
            catch (Exception)
            {
                return Parse("en", Properties.Resources.local);
            }
        }

        public static object Translate(string key)
        {
            string value;
            return _language.Dictionary.TryGetValue(key, out value) ? value : $"{key} not found";
        }

        public static Language Parse(string languageName, string text)
        {
            var language = new Language {TwoLetterLanguageCode = languageName};
            var regex = new Regex(@"^[a-zA-Z][\-_a-zA-Z0-9]*$");

            foreach (var line in text.Split(new[] {Environment.NewLine}, StringSplitOptions.None))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("=")) continue;

                var pair = line.Split(new[] {':'}, 2);
                if (pair.Length != 2) throw new FormatException(ErrorMessage("invalid expression", line));

                var name = pair[0];
                var value = pair[1].Trim();

                if (regex.IsMatch(name) == false) throw new FormatException(ErrorMessage("invalid identifier", line));
                if (string.IsNullOrWhiteSpace(value)) throw new FormatException(ErrorMessage("empty value", line));

                language.Dictionary.Add(name, value);
            }
            return language;
        }

        private static string ErrorMessage(string message, string line)
        {
            return $"{message}: {line}";
        }

        internal class Language
        {
            public string TwoLetterLanguageCode { get; set; }
            public readonly Dictionary<string, string> Dictionary = new Dictionary<string, string>();
        }
    }
}