using System;
using System.IO;
using System.Windows.Documents;
using System.Windows.Input;
using Newtonsoft.Json;

namespace MarkdownEdit
{
    public class InputKeyBindingSettings
    {
        /// <summary>
        /// The windows the Mainwindow binding keys
        /// </summary>
        private static MainWindow _window;

        public static MainWindow MainWindowProperty
        {
            set { _window = value; }
        }

        public static string KeyBindingFile => Path.Combine(UserSettings.SettingsFolder, "key_binding.json");

        /// <summary>
        /// Load key binding settings from file
        /// </summary>
        /// <returns></returns>
        private static InputKeyBinding Load()
        {
            // create default binding file
            if (!File.Exists(KeyBindingFile))
            {
                var binding = new InputKeyBinding();
                Save(binding);
                return binding;
            }

            var bindingFromFile = JsonConvert.DeserializeObject<InputKeyBinding>(File.ReadAllText(KeyBindingFile));
            return bindingFromFile;
        }

        /// <summary>
        /// Only called for create fresh default binding
        /// </summary>
        /// <param name="keyBinding"></param>
        private static void Save(InputKeyBinding keyBinding)
        {
            File.WriteAllText(KeyBindingFile, JsonConvert.SerializeObject(keyBinding, Formatting.Indented));
        }

        public static void Update()
        {
            if (_window == null) return;

            var changed = false;
            var mergedBindings = new InputKeyBinding();
            mergedBindings.PropertyChanged += (sender, args) => changed = true;
            mergedBindings.Merge(Load());

            if (changed)
            {
                _window.Dispatcher.InvokeAsync(() =>
                {
                    var _collection = _window.InputBindings;
                    _collection.Clear();
                    _collection.Add(new KeyBinding(EditingCommands.ToggleBold, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ToggleBold)));
                    _collection.Add(new KeyBinding(EditingCommands.ToggleItalic, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ToggleItalic)));
                    _collection.Add(new KeyBinding(MainWindow.ToggleCodeCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ToggleCode)));
                    _collection.Add(new KeyBinding(ApplicationCommands.SaveAs, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.SaveAs)));
                    _collection.Add(new KeyBinding(MainWindow.ToggleWordWrapCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ToggleWordWrap)));
                    _collection.Add(new KeyBinding(MainWindow.FindNextCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.FindNext)));
                    _collection.Add(new KeyBinding(MainWindow.FindPreviousCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.FindPrevious)));

                    var insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.InsertHeader1));
                    insertHeaderBinding.CommandParameter = "1";
                    _collection.Add(insertHeaderBinding);

                    insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.InsertHeader2));
                    insertHeaderBinding.CommandParameter = "2";
                    _collection.Add(insertHeaderBinding);

                    insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.InsertHeader3));
                    insertHeaderBinding.CommandParameter = "3";
                    _collection.Add(insertHeaderBinding);

                    insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.InsertHeader4));
                    insertHeaderBinding.CommandParameter = "4";
                    _collection.Add(insertHeaderBinding);

                    insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.InsertHeader5));
                    insertHeaderBinding.CommandParameter = "5";
                    _collection.Add(insertHeaderBinding);

                    insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.InsertHeader6));
                    insertHeaderBinding.CommandParameter = "6";
                    _collection.Add(insertHeaderBinding);

                    _collection.Add(new KeyBinding(EditingCommands.IncreaseFontSize, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.IncreaseFontSize)));
                    _collection.Add(new KeyBinding(EditingCommands.DecreaseFontSize, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.DecreaseFontSize)));
                    _collection.Add(new KeyBinding(MainWindow.RestoreFontSizeCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.RestoreFontSize)));
                    _collection.Add(new KeyBinding(MainWindow.OpenUserSnippetsCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.OpenUserSnippets)));
                    _collection.Add(new KeyBinding(MainWindow.OpenUserDictionaryCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.OpenUserDictionary)));
                    _collection.Add(new KeyBinding(MainWindow.ToggleSpellCheckCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ToggleSpellCheck)));
                    _collection.Add(new KeyBinding(MainWindow.OpenUserTemplateCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.OpenUserTemplate)));
                    _collection.Add(new KeyBinding(MainWindow.OpenUserSettingsCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.OpenUserSettings)));
                    _collection.Add(new KeyBinding(MainWindow.OpenKeybindingSettingsCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.OpenKeybindingSettings)));
                    _collection.Add(new KeyBinding(MainWindow.ToggleFullScreenCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ToggleFullScreen)));
                    _collection.Add(new KeyBinding(MainWindow.TogglePreviewCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.TogglePrevious)));
                    _collection.Add(new KeyBinding(MainWindow.RecentFilesCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.RecentFiles)));
                    _collection.Add(new KeyBinding(MainWindow.PasteSpecialCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.PasteSpecial)));
                    _collection.Add(new KeyBinding(MainWindow.ShowThemeDialogCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ShowThemeDialog)));
                    _collection.Add(new KeyBinding(MainWindow.ExportHtmlCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ExportHtml)));
                    _collection.Add(new KeyBinding(MainWindow.ShowGotoLineDialogCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ShowGotoLineDialog)));
                    _collection.Add(new KeyBinding(MainWindow.ToggleAutoSaveCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.ToggleAutoSave)));
                    _collection.Add(new KeyBinding(MainWindow.SelectPreviousHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.SelectPreviousHeader)));
                    _collection.Add(new KeyBinding(MainWindow.SelectNextHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.SelectNextHeader)));
                    _collection.Add(new KeyBinding(MainWindow.OpenNewInstanceCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.OpenNewInstance)));
                    _collection.Add(new KeyBinding(MainWindow.InsertFileCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBindings.InsertFile)));
                });
            }
        }
    }
}