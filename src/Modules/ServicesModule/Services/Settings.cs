using System.Windows;
using System.Windows.Media;
using Infrastructure;
using Jot.DefaultInitializer;
using Prism.Mvvm;

namespace ServicesModule.Services
{
    public class Settings : BindableBase, ISettings
    {
        // Editor
        private FontFamily _font = new FontFamily("Consolas");
        private double _fontSize = 16;
        private bool _wordWrap = true;
        private string _currentFileName = string.Empty;
        private bool _autoSave;
        private bool _showLineNumbers;
        private bool _openLastFile;
        private bool _formatTextOnSave;
        private bool _rememberLastPosition;
        private bool _highlightCurrentLine;
        private bool _showVerticalScrollbar;
        private bool _showSpaces;
        private bool _showLineEndings;
        private bool _showTabs;
        private bool _synchronizeScrollPositions;
        private bool _donated;

        // Spell Check
        private bool _spellCheckEnable = true;
        private bool _spellCheckIgnoreCodeBlocks = true;
        private bool _spellCheckIgnoreMarkupTags = true;
        private bool _spellCheckIgnoreAllCaps = true;
        private bool _spellCheckIgnoreWordsWithDigits = true;
        private string _spellCheckDictionary = "English (United States)";

        // Preview
        private MarkdownEngines _markdownEngine = MarkdownEngines.CommonMark;

        public Settings()
        {
            Globals.Tracker.Configure(this).Apply();
            if (Application.Current.MainWindow != null)
            {
                Application.Current.MainWindow.Closed += (sd, ea) => Globals.Tracker.Configure(this).Persist();
            }
        }

        // === Editor

        [Trackable]
        public string CurrentFileName
        {
            get => _currentFileName;
            set => SetProperty(ref _currentFileName, value);
        }

        [Trackable]
        public FontFamily Font
        {
            get => _font;
            set => SetProperty(ref _font, value);
        }

        [Trackable]
        public double FontSize
        {
            get => _fontSize;
            set => SetProperty(ref _fontSize,  value);
        }

        [Trackable]
        public bool AutoSave
        {
            get => _autoSave;
            set => SetProperty(ref _autoSave, value);
        }

        [Trackable]
        public bool ShowLineNumbers
        {
            get => _showLineNumbers;
            set => SetProperty(ref _showLineNumbers, value);
        }

        [Trackable]
        public bool OpenLastFile
        {
            get => _openLastFile;
            set => SetProperty(ref _openLastFile, value);
        }

        [Trackable]
        public bool FormatTextOnSave
        {
            get => _formatTextOnSave;
            set => SetProperty(ref _formatTextOnSave, value);
        }

        [Trackable]
        public bool RememberLastPosition
        {
            get => _rememberLastPosition;
            set => SetProperty(ref _rememberLastPosition, value);
        }

        [Trackable]
        public bool HighlightCurrentLine
        {
            get => _highlightCurrentLine;
            set => SetProperty(ref _highlightCurrentLine, value);
        }

        [Trackable]
        public bool ShowVerticalScrollbar
        {
            get => _showVerticalScrollbar;
            set => SetProperty(ref _showVerticalScrollbar, value);
        }

        [Trackable]
        public bool ShowSpaces
        {
            get => _showSpaces;
            set => SetProperty(ref _showSpaces, value);
        }

        [Trackable]
        public bool ShowLineEndings
        {
            get => _showLineEndings;
            set => SetProperty(ref _showLineEndings, value);
        }

        [Trackable]
        public bool ShowTabs
        {
            get => _showTabs;
            set => SetProperty(ref _showTabs, value);
        }

        [Trackable]
        public bool SynchronizeScrollPositions
        {
            get => _synchronizeScrollPositions;
            set => SetProperty(ref _synchronizeScrollPositions, value);
        }

        [Trackable]
        public bool WordWrap
        {
            get => _wordWrap;
            set => SetProperty(ref _wordWrap, value);
        }

        // === Spell Check

        [Trackable]
        public bool SpellCheckEnable
        {
            get => _spellCheckEnable;
            set => SetProperty(ref _spellCheckEnable, value);
        }

        [Trackable]
        public bool SpellCheckIgnoreCodeBlocks
        {
            get => _spellCheckIgnoreCodeBlocks;
            set => SetProperty(ref _spellCheckIgnoreCodeBlocks, value);
        }

        [Trackable]
        public bool SpellCheckIgnoreMarkupTags
        {
            get => _spellCheckIgnoreMarkupTags;
            set => SetProperty(ref _spellCheckIgnoreMarkupTags,value);
        }

        [Trackable]
        public bool SpellCheckIgnoreAllCaps
        {
            get => _spellCheckIgnoreAllCaps;
            set => SetProperty(ref _spellCheckIgnoreAllCaps,value);
        }

        [Trackable]
        public bool SpellCheckIgnoreWordsWithDigits
        {
            get => _spellCheckIgnoreWordsWithDigits;
            set => SetProperty(ref _spellCheckIgnoreWordsWithDigits,value);
        }

        [Trackable]
        public string SpellCheckDictionary
        {
            get => _spellCheckDictionary;
            set => SetProperty(ref _spellCheckDictionary, value);
        }

        // === Preview

        [Trackable]
        public MarkdownEngines MarkdownEngines
        {
            get => _markdownEngine;
            set => SetProperty(ref _markdownEngine, value);
        }

        // === Other

        public bool Donated
        {
            get => _donated;
            set => SetProperty(ref _donated, value);
        }
    }
}
