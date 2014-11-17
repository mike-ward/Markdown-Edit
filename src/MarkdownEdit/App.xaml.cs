using System.IO;
using System.Windows;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public partial class App
    {
        private FileSystemWatcher _userSettingsWatcher;

        public static UserSettings UserSettings { get; private set; }

        private void OnStartup(object sender, StartupEventArgs ea)
        {
            InitializeSettings();
            new RemoteManager();
        }

        private void InitializeSettings()
        {
            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();
            }

            UserSettings = UserSettings.Load();

            _userSettingsWatcher = new FileSystemWatcher
            {
                Path = UserSettings.SettingsFolder,
                Filter = Path.GetFileName(UserSettings.SettingsFile),
                NotifyFilter = NotifyFilters.LastWrite
            };
            _userSettingsWatcher.Changed += (s, e) => UserSettings.Update();
            _userSettingsWatcher.EnableRaisingEvents = true;
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}