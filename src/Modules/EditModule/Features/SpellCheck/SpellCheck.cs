using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EditModule.ViewModels;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using Infrastructure;

namespace EditModule.Features.SpellCheck
{
    public class SpellCheck : IEditFeature
    {
        private readonly ISpellCheckService _spellCheckService;
        private readonly ISpellCheckBackgroundRenderer _spellCheckRenderer;
        private readonly ISettings _userSettings;
        private readonly IAbstractSyntaxTree _abstractSyntaxTree;
        private TextEditor _textEditor;

        private readonly Regex _markupTag = new Regex("<(?:\"[^\"]*\"['\"]*|'[^']*'['\"]*|[^'\">])+>", RegexOptions.Compiled);
        private readonly Regex _codeBlock = new Regex(@"^(\s{4,}|\t)[^-*0..9].*");
        private readonly Regex _inlineCode = new Regex(@"`(.*?)`");
        private readonly Regex _markdownUri = new Regex(@"\[([^\[]+)\]\(([^\)]+)\)");

        private readonly Regex _uriFinderRegex = new Regex(
            "(http|ftp|https|mailto):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\.,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?",
            RegexOptions.Compiled);

        private readonly Regex _wordSeparatorRegex = new Regex(
            "-[^\\w]+|^'[^\\w]+|[^\\w]+'[^\\w]+|[^\\w]+-[^\\w]+|[^\\w]+'$|[^\\w]+-$|^-$|^'$|[^\\w'-]",
            RegexOptions.Compiled);

        public SpellCheck(
            ISpellCheckService spellCheckService, 
            ISpellCheckBackgroundRenderer spellCheckRenderer, 
            ISettings userSettings,
            IAbstractSyntaxTree abstractSyntaxTree)
        {
            _spellCheckService = spellCheckService;
            _spellCheckRenderer = spellCheckRenderer;
            _userSettings = userSettings;
            _abstractSyntaxTree = abstractSyntaxTree;
        }

        public void Initialize(EditControlViewModel viewModel)
        {
            _textEditor = viewModel.TextEditor;
            _textEditor.TextArea.TextView.BackgroundRenderers.Add(_spellCheckRenderer);
            _textEditor.TextArea.TextView.VisualLinesChanged += TextViewVisualLinesChanged;
        }

        private void TextViewVisualLinesChanged(object sender, EventArgs e)
        {
            //if (Enabled) DoSpellCheck();
            DoSpellCheck();
        }

        private void DoSpellCheck()
        {
            _spellCheckRenderer.ErrorSegments.Clear();
            IEnumerable<VisualLine> visualLines = _textEditor.TextArea.TextView.VisualLines.AsParallel();
            var abstractSyntaxTree = _abstractSyntaxTree.GenerateAbstractSyntaxTree(_textEditor.Text);

            foreach (var currentLine in visualLines)
            {
                var startIndex = 0;
                var startOfLine = currentLine.FirstDocumentLine.Offset;
                var lengthOfLine = currentLine.LastDocumentLine.EndOffset - startOfLine;

                var originalText = _textEditor.Document.GetText(startOfLine, lengthOfLine);

                originalText = Regex.Replace(originalText, "[\\u2018\\u2019\\u201A\\u201B\\u2032\\u2035]", "'");
                var textWithout = originalText;
                if (_userSettings.SpellCheckIgnoreCodeBlocks)
                {
                    if (!_abstractSyntaxTree.PositionSafeForSmartLink(abstractSyntaxTree, startOfLine, lengthOfLine))
                    {
                        // Generally speaking, if it's not safe to insert a link, it's probably something we don't
                        // want spell checked.
                        continue;
                    }

                    ;
                    if (_codeBlock.IsMatch(originalText))
                    {
                        var firstChar = originalText.FirstOrDefault(c => !char.IsWhiteSpace(c));
                        if (firstChar != '-' && firstChar != '*' && !char.IsDigit(firstChar)) textWithout = "";
                    }
                }

                if (_userSettings.SpellCheckIgnoreMarkupTags) textWithout = _markupTag.Replace(textWithout, "");
                textWithout = _uriFinderRegex.Replace(textWithout, "");
                textWithout = _markdownUri.Replace(textWithout, "");
                if (_userSettings.SpellCheckIgnoreCodeBlocks) textWithout = _inlineCode.Replace(textWithout, "");
                var words = _wordSeparatorRegex.Split(textWithout).Where(s => !string.IsNullOrEmpty(s));
                if (_userSettings.SpellCheckIgnoreAllCaps) words = words.Where(w => w != w.ToUpper()).ToArray();
                if (_userSettings.SpellCheckIgnoreWordsWithDigits) words = words.Where(w => !Regex.Match(w, "\\d").Success).ToArray();
                var errors = 0;

                foreach (var word in words)
                {
                    if (errors >= 20) break;
                    var trimmedWord = word.Trim('\'', '_', '-');

                    var num = currentLine.FirstDocumentLine.Offset
                              + originalText.IndexOf(trimmedWord, startIndex,
                                  StringComparison.InvariantCultureIgnoreCase);

                    if (!_spellCheckService.Check(trimmedWord))
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
    }
}