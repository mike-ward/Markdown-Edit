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
                {"now",  new Snippet {Elements = {new SnippetTextElement {Text = "later"}}}},
                {"post", new Snippet {Elements = {new SnippetTextElement {Text = "---\nlayout: post  \ntitle: ''\n---\n### Programming\n\n### Applications\n\n### Science and Technology\n\n### On the Web\n\n### Stuff I Just Like\n" }}}}
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