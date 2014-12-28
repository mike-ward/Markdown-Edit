using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Newtonsoft.Json;

namespace MarkdownEdit
{
    public static class InputKeyBindingSettings
    {
        private static FileSystemWatcher _keyBindingWatcher;
        public static string KeyBindingFile => Path.Combine(UserSettings.SettingsFolder, "key_bindings.json");

        private static InputKeyBindings Load()
        {
            if (File.Exists(KeyBindingFile) == false) Save(new InputKeyBindings());
            if (_keyBindingWatcher == null)
            {
                Task.Factory.StartNew(() => _keyBindingWatcher = Utility.WatchFile(KeyBindingFile,
                    () => Application.Current.Dispatcher.Invoke(Update)));
            }
            return JsonConvert.DeserializeObject<InputKeyBindings>(File.ReadAllText(KeyBindingFile));
        }

        private static void Save(InputKeyBindings keyBindings)
        {
            File.WriteAllText(KeyBindingFile, JsonConvert.SerializeObject(keyBindings, Formatting.Indented));
        }

        public static void Update()
        {
            var mergedBindings = new InputKeyBindings();
            mergedBindings.Merge(Load());

            var collection = Application.Current.MainWindow.InputBindings;
            collection.Clear();

            Action<ICommand, string> keyBinding = (command, gesture) =>
                collection.Add(new KeyBinding(command, (KeyGesture)new KeyGestureConverter().ConvertFromString(gesture)));

            Action<ICommand, string, object> keyBindingPar = (command, gesture, parameter) =>
                collection.Add(new KeyBinding(command, (KeyGesture)new KeyGestureConverter().ConvertFromString(gesture)) {CommandParameter = parameter});

            keyBinding(EditingCommands.ToggleBold, mergedBindings.ToggleBold);
            keyBinding(EditingCommands.ToggleItalic, mergedBindings.ToggleItalic);
            keyBinding(MainWindow.ToggleCodeCommand, mergedBindings.ToggleCode);
            keyBinding(ApplicationCommands.SaveAs, mergedBindings.SaveAs);
            keyBinding(MainWindow.ToggleWordWrapCommand, mergedBindings.ToggleWordWrap);
            keyBinding(MainWindow.FindNextCommand, mergedBindings.FindNext);
            keyBinding(MainWindow.FindPreviousCommand, mergedBindings.FindPrevious);
            keyBindingPar(MainWindow.InsertHeaderCommand, mergedBindings.InsertHeader1, "1");
            keyBindingPar(MainWindow.InsertHeaderCommand, mergedBindings.InsertHeader2, "2");
            keyBindingPar(MainWindow.InsertHeaderCommand, mergedBindings.InsertHeader3, "3");
            keyBindingPar(MainWindow.InsertHeaderCommand, mergedBindings.InsertHeader4, "4");
            keyBindingPar(MainWindow.InsertHeaderCommand, mergedBindings.InsertHeader5, "5");
            keyBindingPar(MainWindow.InsertHeaderCommand, mergedBindings.InsertHeader6, "6");
            keyBinding(EditingCommands.IncreaseFontSize, mergedBindings.IncreaseFontSize);
            keyBinding(EditingCommands.DecreaseFontSize, mergedBindings.DecreaseFontSize);
            keyBinding(MainWindow.RestoreFontSizeCommand, mergedBindings.RestoreFontSize);
            keyBinding(MainWindow.OpenUserSnippetsCommand, mergedBindings.OpenUserSnippets);
            keyBinding(MainWindow.OpenUserDictionaryCommand, mergedBindings.OpenUserDictionary);
            keyBinding(MainWindow.ToggleSpellCheckCommand, mergedBindings.ToggleSpellCheck);
            keyBinding(MainWindow.OpenUserTemplateCommand, mergedBindings.OpenUserTemplate);
            keyBinding(MainWindow.OpenUserSettingsCommand, mergedBindings.OpenUserSettings);
            keyBinding(MainWindow.OpenKeybindingSettingsCommand, mergedBindings.OpenKeyBindingSettings);
            keyBinding(MainWindow.ToggleFullScreenCommand, mergedBindings.ToggleFullScreen);
            keyBinding(MainWindow.TogglePreviewCommand, mergedBindings.TogglePreview);
            keyBinding(MainWindow.RecentFilesCommand, mergedBindings.RecentFiles);
            keyBinding(MainWindow.PasteSpecialCommand, mergedBindings.PasteSpecial);
            keyBinding(MainWindow.ShowThemeDialogCommand, mergedBindings.ShowThemeDialog);
            keyBinding(MainWindow.ExportHtmlCommand, mergedBindings.ExportHtml);
            keyBinding(MainWindow.ShowGotoLineDialogCommand, mergedBindings.ShowGotoLineDialog);
            keyBinding(MainWindow.ToggleAutoSaveCommand, mergedBindings.ToggleAutoSave);
            keyBinding(MainWindow.SelectPreviousHeaderCommand, mergedBindings.SelectPreviousHeader);
            keyBinding(MainWindow.SelectNextHeaderCommand, mergedBindings.SelectNextHeader);
            keyBinding(MainWindow.OpenNewInstanceCommand, mergedBindings.OpenNewInstance);
            keyBinding(MainWindow.InsertFileCommand, mergedBindings.InsertFile);
        }
    }
}