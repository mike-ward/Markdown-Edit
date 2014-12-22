using ICSharpCode.AvalonEdit.Snippets;

namespace MarkdownEdit
{
    public interface ISnippetManager
    {
        void Load();
        Snippet FindSnippet(string word);
    }
}