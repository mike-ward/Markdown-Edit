using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using Infrastructure;

namespace ServicesModule
{
    public class Strings : IStrings
    {
        private readonly INotify _notify;
        private readonly Dictionary<string, string> _strings;

        // Editor

        public string NewDocumentName => Get("editor-new-document", "New Document");
        public string SaveYourChanges => Get("editor-save-changes", "Save your changes?");

        // Find Replace Dialog

        public string FindReplaceTabFind => Get("find-replace-tab-find", "Find");
        public string FindReplaceTabReplace => Get("find-replace-tab-replace", "Replace");
        public string FindReplaceWatermarkFind => Get("find-replace-watermark-find", "Find");
        public string FindReplaceWatermarkReplace => Get("find-replace-watermark-replace", "Replace with");
        public string FindReplaceFind => Get("find-replace-find", "Find Next");
        public string FindReplaceReplace => Get("find-replace-replace", "Replace");
        public string FindReplaceReplaceAll => Get("find-replace-replace-all", "Replace All");
        public string FindReplaceMatchCase => Get("find-replace-match-case", "Match case");
        public string FindReplaceWholeWord => Get("find-replace-match-whole-word", "Match whole word");
        public string FindReplaceRegularExpression => Get("find-replace-regular-expression", "Regular expression");
        public string FindReplaceWildCards => Get("find-replace-wildcards", "Wild cards");

        public Strings(INotify notify)
        {
            _notify = notify;
            _strings = Load(Thread.CurrentThread.CurrentUICulture);
        }

        private string Get(string key, string fallback)
        {
            return _strings.TryGetValue(key, out var value) ? value : fallback;
        }

        private static string LanguageFolder(CultureInfo cultureInfo)
        {
            var name = cultureInfo.TwoLetterISOLanguageName;
            var path = Path.Combine(Utility.AssemblyFolder(), "Languages", name);
            return path;
        }

        private Dictionary<string, string> Load(CultureInfo cultureInfo)
        {
            try
            {
                var file = LanguageFolder(cultureInfo) + "\\local.txt";
                var text = File.ReadAllText(file);
                return Parse(cultureInfo.TwoLetterISOLanguageName, text);
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }
        private Dictionary<string, string> Parse(string languageName, string text)
        {
            var regex = new Regex(@"^[a-zA-Z][\-_a-zA-Z0-9]*$");
            var lookup = new Dictionary<string, string>();

            foreach (var line in text.Split(new[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                if (line.StartsWith("=")) continue;

                var pair = line.Split(new[] { ':' }, 2);
                if (pair.Length != 2) throw new FormatException(ErrorMessage("invalid expression", line));

                var name = pair[0];
                var value = pair[1].Trim();

                if (regex.IsMatch(name) == false) throw new FormatException(ErrorMessage("invalid identifier", line));
                if (string.IsNullOrWhiteSpace(value)) throw new FormatException(ErrorMessage("empty value", line));

                try
                {
                    lookup.Add(name, value);
                }
                catch (ArgumentException ex)
                {
                    _notify.Alert($"{ex.Message} ({name})");
                }
            }
            return lookup;
        }

        private static string ErrorMessage(string message, string line)
        {
            return $"{message}: {line}";
        }
    }
}
