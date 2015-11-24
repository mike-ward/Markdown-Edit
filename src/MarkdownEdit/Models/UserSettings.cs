using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using static System.Environment;

namespace MarkdownEdit.Models
{
    public class UserSettings : INotifyPropertyChanged, ICloneable, IEquatable<UserSettings>
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
        private bool _ignoreYaml = true;
        private string _spellCheckDictionary = "en_US";
        private bool _spellCheckIgnoreCodeBlocks = true;
        private bool _spellCheckIgnoreAllCaps = true;
        private bool _spellCheckIgnoreMarkupTags = true;
        private bool _spellCheckIgnoreWordsWithDigits = true;
        private int _singlePaneMargin = 16;
        private bool _ignoreTaskbarOnMaximize = true;
        private bool _formatOnSave;
        private bool _githubMarkdown;
        private string _lineEnding = "crlf";
        private string _customMarkdownConverter = "";
        private bool _useDefaultEditor;

        public Theme Theme
        {
            get { return _theme; }
            set { Set(ref _theme, value); }
        }

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

        public int SinglePaneMargin
        {
            get { return _singlePaneMargin; }
            set { Set(ref _singlePaneMargin, value); }
        }

        public bool IgnoreTaskbarOnMaximize
        {
            get { return _ignoreTaskbarOnMaximize; }
            set { Set(ref _ignoreTaskbarOnMaximize, value); }
        }

        public bool FormatOnSave
        {
            get { return _formatOnSave; }
            set { Set(ref _formatOnSave, value); }
        }

        public bool GitHubMarkdown
        {
            get { return _githubMarkdown; }
            set { Set(ref _githubMarkdown, value); }
        }

        public string LineEnding
        {
            get { return _lineEnding; }
            set { Set(ref _lineEnding, value); }
        }

        [JsonIgnore]
        public Tuple<string, string>[] LineEndings => new[]
        {
            new Tuple<string, string>("Windows (CR+LF)", "crlf"),
            new Tuple<string, string>("Unix/Mac (LF)", "lf"),
            new Tuple<string, string>("Apple II (CR)", "cr")
        };

        public string CustomMarkdownConverter
        {
            get { return _customMarkdownConverter; }
            set { Set(ref _customMarkdownConverter, value); }
        }

        public bool UseDefaultEditor
        {
            get { return _useDefaultEditor; }
            set { Set(ref _useDefaultEditor, value); }
        }

        public void Update()
        {
            var userSettings = Load();
            if (userSettings == null) return;

            foreach (var property in GetType().GetProperties())
            {
                var updatedProperty = userSettings.GetType().GetProperty(property.Name);
                if (updatedProperty != null && updatedProperty.CanWrite)
                {
                    property.SetValue(this, updatedProperty.GetValue(userSettings, null), null);
                }
            }
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
                if (File.Exists(SettingsFile)) return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsFile));
                var defaultSettings = new UserSettings {Theme = new Theme()};
                defaultSettings.Save();
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

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // ICloneable
        public object Clone()
        {
            return MemberwiseClone();
        }

        // IEquatable

        public bool Equals(UserSettings other)
        {
            return other != null && GetHashCode() == other.GetHashCode();
        }

        public override int GetHashCode()
        {
            return 17
                ^ EditorFontFamily.GetHashCode()
                ^ EditorFontSize.GetHashCode()
                ^ EditorHighlightCurrentLine.GetHashCode()
                ^ EditorOpenLastCursorPosition.GetHashCode()
                ^ EditorOpenLastFile.GetHashCode()
                ^ EditorShowEndOfLine.GetHashCode()
                ^ EditorShowLineNumbers.GetHashCode()
                ^ EditorShowSpaces.GetHashCode()
                ^ EditorShowTabs.GetHashCode()
                ^ EditorVerticalScrollBarVisible.GetHashCode()
                ^ SynchronizeScrollPositions.GetHashCode()
                ^ SpellCheckDictionary.GetHashCode()
                ^ SpellCheckIgnoreCodeBlocks.GetHashCode()
                ^ SpellCheckIgnoreMarkupTags.GetHashCode()
                ^ SpellCheckIgnoreWordsWithDigits.GetHashCode()
                ^ IgnoreYaml.GetHashCode()
                ^ IgnoreTaskbarOnMaximize.GetHashCode()
                ^ FormatOnSave.GetHashCode()
                ^ GitHubMarkdown.GetHashCode()
                ^ Theme.GetHashCode()
                ^ SinglePaneMargin.GetHashCode()
                ^ LineEnding.GetHashCode()
                ^ CustomMarkdownConverter.GetHashCode()
                ^ UseDefaultEditor.GetHashCode();
        }
    }
}