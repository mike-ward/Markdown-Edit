using ICSharpCode.AvalonEdit.Snippets;

namespace MarkdownEdit
{
    public interface ISnippetManager
    {
        void Initialize();
        Snippet FindSnippet(string word);
    }
}