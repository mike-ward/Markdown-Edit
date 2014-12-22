using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Snippets;

namespace MarkdownEdit
{
    public class SnippetManager : ISnippetManager
    {
        private readonly Dictionary<string, Snippet> _snippets;

        public SnippetManager()
        {
            _snippets = new Dictionary<string, Snippet>
            {
                {"now", new Snippet {Elements = {new SnippetTextElement {Text = "later"}}}}
            };
        }

        public void Load()
        {
        }

        public Snippet FindSnippet(string word)
        {
            Snippet snippet;
            _snippets.TryGetValue(word, out snippet);
            return snippet;
        }
    }
}