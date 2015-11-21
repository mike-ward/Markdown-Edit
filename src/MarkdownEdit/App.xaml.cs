using System.IO;
using System.Threading.Tasks;
using System.Windows;
using MarkdownEdit.Controls;
using MarkdownEdit.Models;
using MarkdownEdit.Properties;
using MarkdownEdit.Snippets;
using MarkdownEdit.SpellCheck;

namespace MarkdownEdit
{
    public partial class App
    {
        public const string Title = "MARKDOWN EDIT";
        private FileSystemWatcher _userSettingsWatcher;
        private ISpellingService _spellingService;

        public static UserSettings UserSettings { get; private set; }

        private void OnStartup(object sender, StartupEventArgs ea)
        {
            InitializeSettings();
            if (UserSettings == null) return;
            Activated += OnActivated;

            _spellingService = new SpellingService();
            var spellCheckProvider = new SpellCheckProvider(_spellingService);
            var snippetManager = new SnippetManager();
            var mainWindow = new MainWindow(spellCheckProvider, snippetManager);
            var windowPlacementSettings = mainWindow.GetWindowPlacementSettings();

            if (windowPlacementSettings.UpgradeSettings)
            {
                windowPlacementSettings.Upgrade();
                windowPlacementSettings.UpgradeSettings = false;
                windowPlacementSettings.Save();
            }

            MainWindow = mainWindow;
            MainWindow.Show();
        }

        private void InitializeSettings()
        {
            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();

                // Adds new settings from this version
                UserSettings.Load()?.Save();
            }

            UserSettings = UserSettings.Load();
            if (UserSettings == null) Shutdown();
        }

        private void OnActivated(object sender, System.EventArgs ea)
        {
            Activated -= OnActivated;
            Task.Factory.StartNew(() =>
            {
                _spellingService.Language = UserSettings.SpellCheckDictionary;
                UserSettings.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(UserSettings.SpellCheckDictionary)) _spellingService.Language = UserSettings.SpellCheckDictionary; };
                _userSettingsWatcher = UserSettings.SettingsFile.WatchFile(UserSettings.Update);
            });
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
            _userSettingsWatcher?.Dispose();
        }
    }
}