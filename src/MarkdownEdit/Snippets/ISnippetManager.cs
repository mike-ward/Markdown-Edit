using ICSharpCode.AvalonEdit.Snippets;

namespace MarkdownEdit.Snippets
{
    public interface ISnippetManager
    {
        void Initialize();

        Snippet FindSnippet(string word);
    }
}