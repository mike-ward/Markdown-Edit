using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit.Snippets;

namespace MarkdownEdit
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
            var file = SnippetFile();

            if (File.Exists(file) == false)
            {
                _snippets.Clear();
                Directory.CreateDirectory(UserSettings.SettingsFolder);
                File.WriteAllText(file, "mde  [Markdown Edit](http://mike-ward.net/markdownedit)");
            }

            _snippets = File.ReadAllLines(file)
                .Where(line => string.IsNullOrWhiteSpace(line) == false)
                .Select(line => line.Split(null, 2))
                .Where(pair => pair.Length == 2)
                .ToDictionary(pair => pair[0], pair => pair[1]);

            if (_snippetFileWatcher == null)
            {
                _snippetFileWatcher = new FileSystemWatcher
                {
                    Path = Path.GetDirectoryName(file),
                    Filter = Path.GetFileName(file),
                    NotifyFilter = NotifyFilters.LastWrite
                };
                _snippetFileWatcher.Changed += (sender, args) => Initialize();
                _snippetFileWatcher.EnableRaisingEvents = true;
            }
        }

        public Snippet FindSnippet(string word)
        {
            string snippetText;
            return _snippets.TryGetValue(word, out snippetText)
                ? new Snippet {Elements = {new SnippetTextElement {Text = ParseSnippetText(snippetText)}}}
                : null;
        }

        private static string ParseSnippetText(string text)
        {
            return text
                .Trim()
                .Replace("\\r", "\r")
                .Replace("\\n", "\n")
                .Replace("\\t", "\t");
        }
    }
}