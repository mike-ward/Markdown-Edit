using System.Collections.Generic;
using ICSharpCode.AvalonEdit.Document;

namespace MarkdownEdit.SpellCheck
{
    public interface ISpellCheckProvider
    {
        void Initialize(Editor editor);
        IEnumerable<TextSegment> GetSpellCheckErrors();
        IEnumerable<string> GetSpellcheckSuggestions(string word);
        void Disconnect();
    }
}