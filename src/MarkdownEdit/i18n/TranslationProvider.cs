using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;

namespace MarkdownEdit.i18n
{
    internal static class TranslationProvider
    {
        // ReSharper disable once InconsistentNaming
        private static readonly Language _language;
        private static string _helpMarkdown;

        static TranslationProvider()
        {
            _language = Load(App.UserSettings == null || string.IsNullOrWhiteSpace(App.UserSettings.CultureLanguage)
                ? Thread.CurrentThread.CurrentUICulture
                : new CultureInfo(App.UserSettings.CultureLanguage));

            //_language = Load(CultureInfo.GetCultureInfo("ru"));
        }

        private static string LanguageFolder(CultureInfo cultureInfo)
        {
            var name = cultureInfo.TwoLetterISOLanguageName;
            var path = Path.Combine(Utility.AssemblyFolder(), "Languages", name);
            return path;
        }

        private static Language Load(CultureInfo cultureInfo)
        {
            try
            {
                var file = LanguageFolder(cultureInfo) + "\\local.txt";
                var text = file.ReadAllTextRetry();
                return Parse(cultureInfo.TwoLetterISOLanguageName, text);
            }
            catch (Exception)
            {
                return Parse("en", Resources.local);
            }
        }

        public static object Translate(string key)
        {
            string value;
            return _language.Dictionary.TryGetValue(key, out value) ? value : $"{key} not found";
        }

        public static string LoadHelp()
        {
            if (_helpMarkdown == null)
            {
                try
                {
                    var path = LanguageFolder(Thread.CurrentThread.CurrentUICulture);
                    //var path = LanguageFolder(CultureInfo.GetCultureInfo("de"));
                    var file = path + "\\help.md";
                    _helpMarkdown = file.ReadAllTextRetry();
                }
                catch (Exception)
                {
                    _helpMarkdown = Resources.Help;
                }
            }
            return _helpMarkdown;
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

        private static string ErrorMessage(string message, string line) { return $"{message}: {line}"; }

        internal class Language
        {
            public readonly Dictionary<string, string> Dictionary = new Dictionary<string, string>();
            public string TwoLetterLanguageCode { get; set; }
        }
    }
}