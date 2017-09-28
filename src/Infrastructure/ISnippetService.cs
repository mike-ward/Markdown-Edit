using ICSharpCode.AvalonEdit.Snippets;

namespace Infrastructure
{
    public interface ISnippetService
    {
        void Initialize();
        Snippet FindSnippet(string word);
    }
}