using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using MarkdownEdit.Properties;
using Newtonsoft.Json;

namespace MarkdownEdit
{
    public class UserSettings : INotifyPropertyChanged
    {
        private Theme _theme;
        private string _editorFontFamily = "Segoe UI";
        private double _editorFontSize = 14;
        private bool _editorOpenLastFile = true;
        private bool _editorVerticalScrollBarVisible;
        private bool _editorShowEndOfLine;
        private bool _editorShowSpaces;
        private bool _editorShowTabs;
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
            EditorFontFamily = userSettings.EditorFontFamily;
            EditorFontSize = userSettings.EditorFontSize;
            Theme = userSettings.Theme;
            EditorOpenLastFile = userSettings.EditorOpenLastFile;
            EditorVerticalScrollBarVisible = userSettings.EditorVerticalScrollBarVisible;
            EditorShowEndOfLine = userSettings.EditorShowEndOfLine;
            EditorShowSpaces = userSettings.EditorShowSpaces;
            EditorShowTabs = userSettings.EditorShowTabs;
        }

        [JsonIgnore]
        public StringCollection RecentFiles
        {
            get { return Settings.Default.RecentFiles ?? (Settings.Default.RecentFiles = new StringCollection()); }
            set
            {
                if (Settings.Default.RecentFiles == value) return;
                Settings.Default.RecentFiles = value;
                OnPropertyChanged();
            }
        }

        public void UpdateRecentFiles(string file)
        {
            var recent = RecentFiles;
            recent.Remove(file);
            recent.Insert(0, file);
            var sc = new StringCollection(); // string collections suck
            sc.AddRange(recent.Cast<string>().Take(20).ToArray());
            RecentFiles = sc;
        }

        // Serialization

        public static string SettingsFolder
        {
            get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Markdown Edit"); }
        }

        public static string SettingsFile
        {
            get { return Path.Combine(SettingsFolder, "user_settings.json"); }
        }

        public void Save()
        {
            Directory.CreateDirectory(SettingsFolder);
            File.WriteAllText(SettingsFile, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static UserSettings Load()
        {
            if (File.Exists(SettingsFile) == false)
            {
                var defaultSettings = new UserSettings { Theme = new Theme() };
                defaultSettings.Save();
            }
            return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsFile));
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(propertyName);
        }
    }
}