using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using ICSharpCode.AvalonEdit.Snippets;
using MarkdownEdit.Models;

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
            var file = SnippetFile();

            if (File.Exists(file) == false)
            {
                Directory.CreateDirectory(UserSettings.SettingsFolder);
                File.WriteAllText(file, Properties.Resources.Snippets);
            }

            _snippets = File.ReadAllLines(file)
                .Where(line => string.IsNullOrWhiteSpace(line) == false)
                .Select(line => line.Split(null, 2))
                .Where(pair => pair.Length == 2)
                .ToDictionary(pair => pair[0], pair => pair[1]);
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
                else if (token == "$CLIPBOARD$") snippet.Elements.Add(new SnippetTextElement { Text = Clipboard.GetText()});
                else if (replaceable.IsMatch(token)) snippet.Elements.Add(new SnippetReplaceableTextElement { Text = token.Trim('$') });
                else snippet.Elements.Add(new SnippetTextElement { Text = token });
            }

            return snippet;
        }
    }
}