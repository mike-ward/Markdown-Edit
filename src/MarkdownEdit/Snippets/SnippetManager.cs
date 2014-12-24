using System.Collections.Generic;
using System.IO;
using System.Linq;
using ICSharpCode.AvalonEdit.Snippets;

namespace MarkdownEdit
{
    public class SnippetManager : ISnippetManager
    {
        private Dictionary<string, string> _snippets;

        public SnippetManager()
        {
            _snippets = new Dictionary<string, string>();
            //{
            //    {"now", "later"},
            //    {"post", "---\nlayout: post  \ntitle: ''\n---\n### Programming\n\n### Applications\n\n### Science and Technology\n\n### On the Web\n\n### Stuff I Just Like\n"}
            //};
        }

        public static string SnippetFile()
        {
            return Path.Combine(UserSettings.SettingsFolder, "snippets.txt");
        }

        public void Load()
        {
            var file = SnippetFile();

            if (File.Exists(file) == false)
            {
                _snippets.Clear();
                Directory.CreateDirectory(UserSettings.SettingsFolder);
                File.WriteAllText(file, "mde  [Markdown Edit](http://mike-ward.net)");
            }

            _snippets = File.ReadAllLines(file)
                .Where(line => !string.IsNullOrWhiteSpace(line))
                .Select(line => line.Split(null, 2))
                .Where(pair => pair.Length == 2)
                .ToDictionary(pair => pair[0], pair => pair[1]);
        }

        public Snippet FindSnippet(string word)
        {
            string snippetText;
            return _snippets.TryGetValue(word, out snippetText) 
                ? new Snippet {Elements = {new SnippetTextElement {Text = snippetText}}} 
                : null;
        }
    }
}