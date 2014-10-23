using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace MarkdownEdit.SpellCheck
{
    public class SpellCheckProvider : ISpellCheckProvider
    {
        private readonly Regex _wordSeparatorRegex = new Regex("-[^\\w]+|^'[^\\w]+|[^\\w]+'[^\\w]+|[^\\w]+-[^\\w]+|[^\\w]+'$|[^\\w]+-$|^-$|^'$|[^\\w'-]", RegexOptions.Compiled);
        private readonly Regex _uriFinderRegex = new Regex("(http|ftp|https|mailto):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?", RegexOptions.Compiled);

        private readonly ISpellingService _spellingService;
        private readonly SpellCheckBackgroundRenderer _spellCheckRenderer;
        private Editor _editor;

        public SpellCheckProvider(ISpellingService spellingService)
        {
            _spellingService = spellingService;
            _spellCheckRenderer = new SpellCheckBackgroundRenderer();
        }

        public void Initialize(Editor editor)
        {
            _editor = editor;
            _editor.EditBox.TextArea.TextView.BackgroundRenderers.Add(_spellCheckRenderer);
            _editor.EditBox.TextArea.TextView.VisualLinesChanged += TextViewVisualLinesChanged;
        }

        public void Disconnect()
        {
            if (_editor == null) return;
            ClearSpellCheckErrors();
            _editor.EditBox.TextArea.TextView.BackgroundRenderers.Remove(_spellCheckRenderer);
            _editor.EditBox.TextArea.TextView.VisualLinesChanged -= TextViewVisualLinesChanged;
            _editor = null;
        }

        private void TextViewVisualLinesChanged(object sender, EventArgs e)
        {
            DoSpellCheck();
        }

        private void DoSpellCheck()
        {
            if (_editor == null) return;
            if (!_editor.EditBox.TextArea.TextView.VisualLinesValid) return;
            _spellCheckRenderer.ErrorSegments.Clear();
            IEnumerable<VisualLine> visualLines = _editor.EditBox.TextArea.TextView.VisualLines.AsParallel();

            foreach (var currentLine in visualLines)
            {
                var startIndex = 0;

                var originalText = _editor.EditBox.Document.GetText(currentLine.FirstDocumentLine.Offset,
                    currentLine.LastDocumentLine.EndOffset - currentLine.FirstDocumentLine.Offset);

                originalText = Regex.Replace(originalText, "[\\u2018\\u2019\\u201A\\u201B\\u2032\\u2035]", "'");

                var textWithoutUrls = _uriFinderRegex.Replace(originalText, "");

                var query = _wordSeparatorRegex.Split(textWithoutUrls).Where(s => !string.IsNullOrEmpty(s));

                foreach (var word in query)
                {
                    var trimmedWord = word.Trim('\'', '_', '-');

                    var num = currentLine.FirstDocumentLine.Offset
                              + originalText.IndexOf(trimmedWord, startIndex, StringComparison.InvariantCultureIgnoreCase);

                    if (!_spellingService.Spell(trimmedWord))
                    {
                        var textSegment = new TextSegment
                        {
                            StartOffset = num,
                            Length = word.Length
                        };
                        _spellCheckRenderer.ErrorSegments.Add(textSegment);
                    }

                    startIndex = originalText.IndexOf(word, startIndex, StringComparison.InvariantCultureIgnoreCase) + word.Length;
                }
            }
        }

        private void ClearSpellCheckErrors()
        {
            if (_spellCheckRenderer == null) return;
            _spellCheckRenderer.ErrorSegments.Clear();
        }

        public IEnumerable<TextSegment> GetSpellCheckErrors()
        {
            if (_spellCheckRenderer == null) return Enumerable.Empty<TextSegment>();
            return _spellCheckRenderer.ErrorSegments;
        }

        public IEnumerable<string> GetSpellcheckSuggestions(string word)
        {
            if (_spellCheckRenderer == null) return Enumerable.Empty<string>();
            return _spellingService.Suggestions(word);
        }
    }
}