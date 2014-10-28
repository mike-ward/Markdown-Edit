using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace MarkdownEdit
{
    public class UserSettings : INotifyPropertyChanged
    {
        private string _editorBackground = "#F7F4EF";
        private string _editorForeground = "Black";
        private string _editorFontFamily = "Consolas";
        private double _editorFontSize = 14;

        public void Update()
        {
            var userSettings = Load();
            EditorBackground = userSettings.EditorBackground;
            EditorForeground = userSettings.EditorForeground;
            EditorFontFamily = userSettings.EditorFontFamily;
            EditorFontSize = userSettings.EditorFontSize;
        }

        public string EditorBackground
        {
            get { return _editorBackground; }
            set
            {
                if (_editorBackground == value) return;
                _editorBackground = value;
                OnPropertyChanged();
            }
        }

        public string EditorForeground
        {
            get { return _editorForeground; }
            set
            {
                if (_editorForeground == value) return;
                _editorForeground = value;
                OnPropertyChanged();
            }
        }

        public string EditorFontFamily
        {
            get { return _editorFontFamily; }
            set
            {
                if (_editorFontFamily == value) return;
                _editorFontFamily = value;
                OnPropertyChanged();
            }
        }

        public double EditorFontSize
        {
            get { return _editorFontSize; }
            set
            {
                if (Math.Abs(_editorFontSize - value) < .000001) return;
                _editorFontSize = value;
                OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public StringCollection RecentFiles
        {
            get { return Properties.Settings.Default.RecentFiles ?? (Properties.Settings.Default.RecentFiles = new StringCollection()); }
            set
            {
                if (Properties.Settings.Default.RecentFiles == value) return;
                Properties.Settings.Default.RecentFiles = value;
                OnPropertyChanged();
            }
        }

        public void UpdateRecentFiles(string file)
        {
            var recent = RecentFiles;
            recent.Remove(file);
            recent.Add(file);
            RecentFiles = recent;
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
            if (File.Exists(SettingsFile)) return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(SettingsFile));
            new UserSettings().Save();
            return Load();
        }

        // INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}