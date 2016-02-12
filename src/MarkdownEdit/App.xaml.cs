using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
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

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        public static UserSettings UserSettings { get; private set; }

        private void OnStartup(object sender, StartupEventArgs ea)
        {
            if (AlreadyEditingFileInProcess())
            {
                Shutdown();
                return;
            }

            InitializeSettings();

            if (UserSettings == null)
            {
                Shutdown();
                return;
            }

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

        private static bool AlreadyEditingFileInProcess()
        {
            var fileName = Environment.GetCommandLineArgs().Skip(1).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            var currentProcess = Process.GetCurrentProcess();

            foreach (var process in Process.GetProcessesByName(currentProcess.ProcessName)
                .Where(p => p.Id != currentProcess.Id)
                .Where(p => OldSchool.IsEditingFile(p, fileName)))
            {
                if (IsIconic(process.MainWindowHandle)) ShowWindowAsync(process.MainWindowHandle, 9 /* SW_RESTORE */);
                SetForegroundWindow(process.MainWindowHandle);
                return true;
            }

            return false;
        }

        private static void InitializeSettings()
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
        }

        private void OnActivated(object sender, EventArgs ea)
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