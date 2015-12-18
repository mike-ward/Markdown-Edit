using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using ICSharpCode.AvalonEdit.Snippets;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;

namespace MarkdownEdit.Snippets
{
    public class SnippetManager : ISnippetManager
    {
        private Dictionary<string, string> _snippets;
        private FileSystemWatcher _snippetFileWatcher;

        public SnippetManager()
        {
            _snippets = new Dictionary<string, string>();
        }

        public static string SnippetFile()
        {
            return Path.Combine(UserSettings.SettingsFolder, "snippets.txt");
        }

        public void Initialize()
        {
            ReadSnippetFile();
            if (_snippetFileWatcher == null) _snippetFileWatcher = SnippetFile().WatchFile(ReadSnippetFile);
        }

        private void ReadSnippetFile()
        {
            try
            {
                var file = SnippetFile();

                if (File.Exists(file) == false)
                {
                    Directory.CreateDirectory(UserSettings.SettingsFolder);
                    File.WriteAllText(file, Resources.Snippets);
                }

                var key = default(string);
                var value = default(string);
                _snippets = new Dictionary<string, string>();

                foreach (var line in File.ReadAllLines(file))
                {
                    if (key != null)
                    {
                        // Keep reading until terminator sequence
                        if (line.TrimEnd() != "::")
                        {
                            value += line + Environment.NewLine;
                        }
                        else
                        {
                            _snippets.Add(key, value);
                            key = null;
                            value = null;
                        }
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var pair = line.Split(null, 2);

                    if (pair.Length == 2)
                    {
                        _snippets.Add(pair[0], pair[1]);
                    }

                    else if (pair[0].EndsWith("::") && pair[0][0] != ':')
                    {
                        // begin multiline sequence
                        key = pair[0].TrimEnd(':');
                        value = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Utility.Alert($"{ex.Message} in {SnippetFile()}");
            }
        }

        public Snippet FindSnippet(string word)
        {
            string snippetText;
            return _snippets.TryGetValue(word, out snippetText)
                ? BuildSnippet(snippetText)
                : null;
        }

        private static Snippet BuildSnippet(string text)
        {
            var expanded = text
                .Trim()
                .Replace("\\r", "\r")
                .Replace("\\n", "\n")
                .Replace("\\t", "\t")
                .ReplaceDate();

            var snippet = new Snippet();
            var replaceable = new Regex(@"(\$\w+\$)");
            foreach (var token in replaceable.Split(expanded))
            {
                if (token == "$END$") snippet.Elements.Add(new SnippetCaretElement());
                else if (token == "$CLIPBOARD$") snippet.Elements.Add(new SnippetTextElement {Text = Clipboard.GetText()});
                else if (replaceable.IsMatch(token)) snippet.Elements.Add(new SnippetReplaceableTextElement {Text = token.Trim('$')});
                else snippet.Elements.Add(new SnippetTextElement {Text = token});
            }

            return snippet;
        }
    }
}