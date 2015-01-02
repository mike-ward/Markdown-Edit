using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Snippets;

namespace MarkdownEdit
{
    public class SnippetManager : ISnippetManager
    {
        private Dictionary<string, string> _snippets;
        // ReSharper disable once NotAccessedField.Local
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
            _snippetFileWatcher = Utility.WatchFile(SnippetFile(), ReadSnippetFile);
        }

        private void ReadSnippetFile()
        {
            var file = SnippetFile();

            if (File.Exists(file) == false)
            {
                _snippets.Clear();
                Directory.CreateDirectory(UserSettings.SettingsFolder);
                File.WriteAllText(file, "date  $DATE(\"f\")$");
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
            var matches = Regex.Split(expanded, @"(\$\w+\$)");
            foreach (var match in matches)
            {
                if (match == "$END$") snippet.Elements.Add(new SnippetCaretElement());
                else if (Regex.IsMatch(match, @"(\$\w+\$)")) snippet.Elements.Add(new SnippetReplaceableTextElement {Text = match.Trim('$')});
                else snippet.Elements.Add(new SnippetTextElement {Text = match});
            }

            return snippet;
        }
    }
}