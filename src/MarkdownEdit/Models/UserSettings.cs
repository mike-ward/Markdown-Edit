using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Environment;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace MarkdownEdit
{
    public class UserSettings : INotifyPropertyChanged
    {
        private Theme _theme;
        private string _editorFontFamily = "Consolas";
        private double _editorFontSize = 15.0;
        private bool _editorOpenLastFile = true;
        private bool _editorOpenLastCursorPosition = true;
        private bool _editorVerticalScrollBarVisible = true;
        private bool _editorShowEndOfLine;
        private bool _editorShowSpaces;
        private bool _editorShowTabs;
        private bool _editorShowLineNumbers;
        private bool _editorHighlightCurrentLine;
        private bool _synchronizeScrollPositions = true;
        private bool _ignoreYaml;
        private string _spellCheckDictionary = "en_US";
        private bool _spellCheckIgnoreCodeBlocks = true;
        private bool _spellCheckIgnoreAllCaps = true;
        private bool _spellCheckIgnoreMarkupTags = true;
        private bool _spellCheckIgnoreWordsWithDigits = true;

        public string EditorFontFamily
        {
            get { return _editorFontFamily; }
            set { Set(ref _editorFontFamily, value); }
        }

        public double EditorFontSize
        {
            get { return _editorFontSize; }
            set { Set(ref _editorFontSize, value); }
        }

        public bool EditorOpenLastFile
        {
            get { return _editorOpenLastFile; }
            set { Set(ref _editorOpenLastFile, value); }
        }

        public bool EditorOpenLastCursorPosition
        {
            get { return _editorOpenLastCursorPosition; }
            set { Set(ref _editorOpenLastCursorPosition, value); }
        }

        public bool EditorVerticalScrollBarVisible
        {
            get { return _editorVerticalScrollBarVisible; }
            set { Set(ref _editorVerticalScrollBarVisible, value); }
        }

        public bool EditorShowEndOfLine
        {
            get { return _editorShowEndOfLine; }
            set { Set(ref _editorShowEndOfLine, value); }
        }

        public bool EditorShowSpaces
        {
            get { return _editorShowSpaces; }
            set { Set(ref _editorShowSpaces, value); }
        }

        public bool EditorShowTabs
        {
            get { return _editorShowTabs; }
            set { Set(ref _editorShowTabs, value); }
        }

        public bool EditorShowLineNumbers
        {
            get { return _editorShowLineNumbers; }
            set { Set(ref _editorShowLineNumbers, value); }
        }

        public bool EditorHighlightCurrentLine
        {
            get { return _editorHighlightCurrentLine; }
            set { Set(ref _editorHighlightCurrentLine, value); }
        }

        public bool SynchronizeScrollPositions
        {
            get { return _synchronizeScrollPositions; }
            set { Set(ref _synchronizeScrollPositions, value); }
        }

        public bool IgnoreYaml
        {
            get { return _ignoreYaml; }
            set { Set(ref _ignoreYaml, value); }
        }

        public string SpellCheckDictionary
        {
            get { return _spellCheckDictionary; }
            set { Set(ref _spellCheckDictionary, value); }
        }

        public bool SpellCheckIgnoreCodeBlocks
        {
            get { return _spellCheckIgnoreCodeBlocks; }
            set { Set(ref _spellCheckIgnoreCodeBlocks, value); }
        }

        public bool SpellCheckIgnoreAllCaps
        {
            get { return _spellCheckIgnoreAllCaps; }
            set { Set(ref _spellCheckIgnoreAllCaps, value); }
        }

        public bool SpellCheckIgnoreMarkupTags
        {
            get { return _spellCheckIgnoreMarkupTags; }
            set { Set(ref _spellCheckIgnoreMarkupTags, value); }
        }

        public bool SpellCheckIgnoreWordsWithDigits
        {
            get { return _spellCheckIgnoreWordsWithDigits; }
            set { Set(ref _spellCheckIgnoreWordsWithDigits, value); }
        }

        public Theme Theme
        {
            get { return _theme; }
            set { Set(ref _theme, value); }
        }

        public void Update()
        {
            var userSettings = Load();
            if (userSettings == null) return;
            EditorFontFamily = userSettings.EditorFontFamily;
            EditorFontSize = userSettings.EditorFontSize;
            Theme = userSettings.Theme;
            EditorOpenLastFile = userSettings.EditorOpenLastFile;
            EditorVerticalScrollBarVisible = userSettings.EditorVerticalScrollBarVisible;
            EditorShowEndOfLine = userSettings.EditorShowEndOfLine;
            EditorShowSpaces = userSettings.EditorShowSpaces;
            EditorShowTabs = userSettings.EditorShowTabs;
            EditorShowLineNumbers = userSettings.EditorShowLineNumbers;
            EditorHighlightCurrentLine = userSettings.EditorHighlightCurrentLine;
            SynchronizeScrollPositions = userSettings.SynchronizeScrollPositions;
            IgnoreYaml = userSettings.IgnoreYaml;
            SpellCheckDictionary = userSettings.SpellCheckDictionary;
            SpellCheckIgnoreAllCaps = userSettings.SpellCheckIgnoreAllCaps;
            SpellCheckIgnoreCodeBlocks = userSettings.SpellCheckIgnoreCodeBlocks;
            SpellCheckIgnoreMarkupTags = userSettings.SpellCheckIgnoreMarkupTags;
            SpellCheckIgnoreWordsWithDigits = userSettings.SpellCheckIgnoreWordsWithDigits;
        }

        // Serialization

        public static string SettingsFolder => Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "Markdown Edit");

        public static string SettingsFile => Path.Combine(SettingsFolder, "user_settings.json");

        public void Save()
        {
            Directory.CreateDirectory(SettingsFolder);
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static UserSettings Load()
        {
            try
            {
                if (File.Exists(SettingsFile) == false)
                {
                    var defaultSettings = new UserSettings {Theme = new Theme()};
                    defaultSettings.Save();
                }
                return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsFile));
            }
            catch (Exception ex)
            {
                Utility.ShowParseError(ex, SettingsFile);
                return null;
            }
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            OnPropertyChanged(propertyName);
        }
    }
}