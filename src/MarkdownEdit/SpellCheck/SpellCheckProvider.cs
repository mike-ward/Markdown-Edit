using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using MarkdownEdit.Controls;
using MarkdownEdit.Models;

namespace MarkdownEdit.SpellCheck
{
    public class SpellCheckProvider : ISpellCheckProvider
    {
        private readonly Regex _codeBlock = new Regex(@"^(\s{4,}|\t)[^-*0..9].*");
        private readonly Regex _inlineCode = new Regex(@"`(.*?)`");
        private readonly Regex _mardownUri = new Regex(@"\[([^\[]+)\]\(([^\)]+)\)");

        private readonly Regex _markupTag = new Regex("<(?:\"[^\"]*\"['\"]*|'[^']*'['\"]*|[^'\">])+>",
            RegexOptions.Compiled);

        private readonly SpellCheckBackgroundRenderer _spellCheckRenderer;

        private readonly ISpellingService _spellingService;

        private readonly Regex _uriFinderRegex =
            new Regex(
                "(http|ftp|https|mailto):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?",
                RegexOptions.Compiled);

        private readonly Regex _wordSeparatorRegex =
            new Regex("-[^\\w]+|^'[^\\w]+|[^\\w]+'[^\\w]+|[^\\w]+-[^\\w]+|[^\\w]+'$|[^\\w]+-$|^-$|^'$|[^\\w'-]",
                RegexOptions.Compiled);

        private Editor _editor;
        private bool _enabled;

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

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                if (_enabled == false) ClearSpellCheckErrors();
            }
        }

        public string CustomDictionaryFile() { return _spellingService.CustomDictionaryFile(); }

        public void Disconnect()
        {
            if (_editor == null) return;
            ClearSpellCheckErrors();
            _editor.EditBox.TextArea.TextView.BackgroundRenderers.Remove(_spellCheckRenderer);
            _editor.EditBox.TextArea.TextView.VisualLinesChanged -= TextViewVisualLinesChanged;
            _editor = null;
        }

        public string[] Languages() { return _spellingService?.Languages() ?? new string[0]; }

        public ISpellingService SpellingService() { return _spellingService; }

        public IEnumerable<TextSegment> GetSpellCheckErrors() =>
            _spellCheckRenderer == null ? Enumerable.Empty<TextSegment>() : _spellCheckRenderer.ErrorSegments;

        public IEnumerable<string> GetSpellCheckSuggestions(string word) =>
            _spellCheckRenderer == null ? Enumerable.Empty<string>() : _spellingService.Suggestions(word);

        public void Add(string word)
        {
            if (string.IsNullOrWhiteSpace(word) || _spellingService == null || !Enabled) return;
            _spellingService.Add(word);
        }

        private void TextViewVisualLinesChanged(object sender, EventArgs e)
        {
            if (Enabled) DoSpellCheck();
        }

        private void DoSpellCheck()
        {
            if (_editor == null) return;
            if (!_editor.EditBox.TextArea.TextView.VisualLinesValid) return;
            var userSettings = App.UserSettings;
            _spellCheckRenderer.ErrorSegments.Clear();
            IEnumerable<VisualLine> visualLines = _editor.EditBox.TextArea.TextView.VisualLines.AsParallel();

            foreach (var currentLine in visualLines)
            {
                var startIndex = 0;
                var startOfLine = currentLine.FirstDocumentLine.Offset;
                var lengthOfLine = currentLine.LastDocumentLine.EndOffset - startOfLine;

                var originalText = _editor.EditBox.Document.GetText(startOfLine, lengthOfLine);

                originalText = Regex.Replace(originalText, "[\\u2018\\u2019\\u201A\\u201B\\u2032\\u2035]", "'");
                var textWithout = originalText;
                if (userSettings.SpellCheckIgnoreCodeBlocks)
                {
                    if (!AbstractSyntaxTree.PositionSafeForSmartLink(_editor.AbstractSyntaxTree, startOfLine, lengthOfLine))
                    {
                        // Generally speaking, if it's not safe to insert a link, it's probably something we don't
                        // want spell checked.
                        continue;
                    };
                    if (_codeBlock.IsMatch(originalText))
                    {
                        var firstChar = originalText.FirstOrDefault(c => !char.IsWhiteSpace(c));
                        if (firstChar != '-' && firstChar != '*' && !char.IsDigit(firstChar)) textWithout = "";
                    }
                }
                if (userSettings.SpellCheckIgnoreMarkupTags) textWithout = _markupTag.Replace(textWithout, "");
                textWithout = _uriFinderRegex.Replace(textWithout, "");
                textWithout = _mardownUri.Replace(textWithout, "");
                if (userSettings.SpellCheckIgnoreCodeBlocks) textWithout = _inlineCode.Replace(textWithout, "");
                var words = _wordSeparatorRegex.Split(textWithout).Where(s => !string.IsNullOrEmpty(s));
                if (userSettings.SpellCheckIgnoreAllCaps) words = words.Where(w => w != w.ToUpper()).ToArray();
                if (userSettings.SpellCheckIgnoreWordsWithDigits) words = words.Where(w => !Regex.Match(w, "\\d").Success).ToArray();
                var errors = 0;

                foreach (var word in words)
                {
                    if (errors >= 20) break;
                    var trimmedWord = word.Trim('\'', '_', '-');

                    var num = currentLine.FirstDocumentLine.Offset
                              + originalText.IndexOf(trimmedWord, startIndex,
                                  StringComparison.InvariantCultureIgnoreCase);

                    if (!_spellingService.Spell(trimmedWord))
                    {
                        var textSegment = new TextSegment {StartOffset = num, Length = word.Length};
                        _spellCheckRenderer.ErrorSegments.Add(textSegment);
                        errors += 1;
                    }

                    startIndex = originalText.IndexOf(word, startIndex, StringComparison.InvariantCultureIgnoreCase)
                                 + word.Length;
                }
            }
        }

        private void ClearSpellCheckErrors() => _spellCheckRenderer?.ErrorSegments.Clear();
    }
}