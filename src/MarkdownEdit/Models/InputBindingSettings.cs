using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Environment;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace MarkdownEdit
{
    public class InputBindingSettings : INotifyPropertyChanged
    {
        /// <summary>
        /// The windows the Mainwindow binding keys
        /// </summary>
        private static MainWindow _window = null;
        public static MainWindow MainWindowProperty{
            set { _window = value; }
        }

        public static string SettingsFolder => Path.Combine(GetFolderPath(SpecialFolder.ApplicationData), "Markdown Edit");

        public static string KeyBindingFile => Path.Combine(SettingsFolder, "key_binding.json");

        /// <summary>
        /// Load key binding settings from file and merge with default bindings
        /// </summary>
        /// <returns></returns>
        public static InputBindingSettings Load(MainWindow window = null)
        {
            if (window!= null)
            {
                InputBindingSettings.MainWindowProperty = window;
            }
            if (!File.Exists(KeyBindingFile))
            {
                var binding = new InputBindingSettings();
                binding.Save();
                return binding;
            }

            var bindingFromFile= JsonConvert.DeserializeObject<InputBindingSettings>(File.ReadAllText(KeyBindingFile));
            return bindingFromFile;
        }

        public void Save()
        {
            File.WriteAllText(KeyBindingFile , JsonConvert.SerializeObject(this, Formatting.Indented));
        }

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

        public void Update()
        {
            var bindingsFromFile = InputBindingSettings.Load();
            _window.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate(){
                var _collection = _window.InputBindings;
                _collection.Clear();
                _collection.Add(new KeyBinding(EditingCommands.ToggleBold, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ToggleBold)));
                _collection.Add(new KeyBinding(EditingCommands.ToggleItalic, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ToggleItalic)));
                _collection.Add(new KeyBinding(MainWindow.ToggleCodeCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ToggleCodeCommand)));
                _collection.Add(new KeyBinding(ApplicationCommands.SaveAs, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.SaveAs)));
                _collection.Add(new KeyBinding(MainWindow.ToggleWordWrapCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ToggleWordWrapCommand)));
                _collection.Add(new KeyBinding(MainWindow.FindNextCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.FindNextCommand)));
                _collection.Add(new KeyBinding(MainWindow.FindPreviousCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.FindPreviousCommand)));

                var insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.InsertHeaderCommand1));
                insertHeaderBinding.CommandParameter = "1";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.InsertHeaderCommand2));
                insertHeaderBinding.CommandParameter = "2";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.InsertHeaderCommand3));
                insertHeaderBinding.CommandParameter = "3";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.InsertHeaderCommand4));
                insertHeaderBinding.CommandParameter = "4";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.InsertHeaderCommand5));
                insertHeaderBinding.CommandParameter = "5";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.InsertHeaderCommand6));
                insertHeaderBinding.CommandParameter = "6";
                _collection.Add(insertHeaderBinding);

                _collection.Add(new KeyBinding(EditingCommands.IncreaseFontSize, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.IncreaseFontSize)));
                _collection.Add(new KeyBinding(EditingCommands.DecreaseFontSize, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.DecreaseFontSize)));
                _collection.Add(new KeyBinding(MainWindow.RestoreFontSizeCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.RestoreFontSizeCommand)));
                _collection.Add(new KeyBinding(MainWindow.OpenUserSnippetsCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.OpenUserSnippetsCommand)));
                _collection.Add(new KeyBinding(MainWindow.OpenUserDictionaryCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.OpenUserDictionaryCommand)));
                _collection.Add(new KeyBinding(MainWindow.ToggleSpellCheckCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ToggleSpellCheckCommand)));
                _collection.Add(new KeyBinding(MainWindow.OpenUserTemplateCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.OpenUserTemplateCommand)));
                _collection.Add(new KeyBinding(MainWindow.OpenUserSettingsCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.OpenUserSettingsCommand)));
                _collection.Add(new KeyBinding(MainWindow.ToggleFullScreenCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ToggleFullScreenCommand)));
                _collection.Add(new KeyBinding(MainWindow.TogglePreviewCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.TogglePreviousCommand)));
                _collection.Add(new KeyBinding(MainWindow.RecentFilesCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.RecentFilesCommand)));
                _collection.Add(new KeyBinding(MainWindow.PasteSpecialCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.PasteSpecialCommand)));
                _collection.Add(new KeyBinding(MainWindow.ShowThemeDialogCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ShowThemeDialogcommand)));
                _collection.Add(new KeyBinding(MainWindow.ExportHtmlCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ExportHtmlCommand)));
                _collection.Add(new KeyBinding(MainWindow.ShowGotoLineDialogCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ShowGotoLineDialogCommand)));
                _collection.Add(new KeyBinding(MainWindow.ToggleAutoSaveCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.ToggleAutoSaveCommand)));
                _collection.Add(new KeyBinding(MainWindow.SelectPreviousHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.SelectPreviousHeaderCommand)));
                _collection.Add(new KeyBinding(MainWindow.SelectNextHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.SelectNextHeaderCommand)));
                _collection.Add(new KeyBinding(MainWindow.OpenNewInstanceCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.OpenNewInstanceCommand)));
                _collection.Add(new KeyBinding(MainWindow.InsertFileCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(bindingsFromFile.InsertFileCommand)));
            });
        }

        #region Property
        private string _togglebold = "ctrl+b";
        public string ToggleBold
        {
            get { return _togglebold; }
            set { Set(ref _togglebold, value); }
        }

        private string _toggleitalic = "ctrl+i";
        public string ToggleItalic
        {
            get { return _toggleitalic; }
            set { Set(ref _toggleitalic, value); }
        }

        private string _togglecodecommand = "ctrl+k";
        public string ToggleCodeCommand
        {
            get { return _togglecodecommand; }
            set { Set(ref _togglecodecommand, value); }
        }

        private string _saveas = "ctrl+shift+s";
        public string SaveAs
        {
            get { return _saveas; }
            set { Set(ref _saveas, value); }
        }

        private string _togglewordwrapcommand = "ctrl+w";
        public string ToggleWordWrapCommand
        {
            get { return _togglewordwrapcommand; }
            set { Set(ref _togglewordwrapcommand, value);}
        }

        private string _findnextcommand = "f3";
        public string FindNextCommand 
        {
            get { return _findnextcommand; }
            set { Set(ref _findnextcommand, value);}
        }

        private string _findpreviouscommand = "shift+f3";
        public string FindPreviousCommand 
        {
            get { return _findpreviouscommand; }
            set { Set(ref _findpreviouscommand, value);}
        }

        private string _insertheadercommand1 = "ctrl+1";
        public string InsertHeaderCommand1
        {
            get { return _insertheadercommand1; }
            set { Set(ref _insertheadercommand1, value);}
        }

        private string _insertheadercommand2 = "ctrl+2";
        public string InsertHeaderCommand2
        {
            get { return _insertheadercommand2; }
            set { Set(ref _insertheadercommand2, value);}
        }
        
        private string _insertheadercommand3 = "ctrl+3";
        public string InsertHeaderCommand3
        {
            get { return _insertheadercommand3; }
            set { Set(ref _insertheadercommand3, value);}
        }

        private string _insertheadercommand4 = "ctrl+4";
        public string InsertHeaderCommand4
        {
            get { return _insertheadercommand4; }
            set { Set(ref _insertheadercommand4, value);}
        }

        private string _insertheadercommand5 = "ctrl+5";
        public string InsertHeaderCommand5
        {
            get { return _insertheadercommand5; }
            set { Set(ref _insertheadercommand5, value);}
        }

        private string _insertheadercommand6 = "ctrl+6";
        public string InsertHeaderCommand6
        {
            get { return _insertheadercommand6; }
            set { Set(ref _insertheadercommand6, value);}
        }

        private string _increasefontsize = "ctrl+plus";
        public string IncreaseFontSize 
        {
            get { return _increasefontsize; }
            set { Set(ref _increasefontsize, value);}
        }

        private string _decreasefontsize = "ctrl+minus";
        public string DecreaseFontSize 
        {
            get { return _decreasefontsize; }
            set { Set(ref _decreasefontsize, value);}
        }

        private string _restorefontsizecommand = "ctrl+0";
        public string RestoreFontSizeCommand
        {
            get { return _restorefontsizecommand; }
            set { Set(ref _restorefontsizecommand, value);}
        }

        private string _openusersnippetscommand = "f6";
        public string OpenUserSnippetsCommand 
        {
            get { return _openusersnippetscommand ; }
            set { Set(ref _openusersnippetscommand, value);}
        }

        private string _openuserdictionarycommand = "f7";
        public string OpenUserDictionaryCommand
        {
            get { return _openuserdictionarycommand; }
            set { Set(ref _openuserdictionarycommand, value);}
        }

        private string _togglespellcheckcommand = "ctrl+f7";
        public string ToggleSpellCheckCommand 
        {
            get { return _togglespellcheckcommand; }
            set { Set(ref _togglespellcheckcommand, value);}
        }

        private string _openusertemplatecommand = "f8";
        public string OpenUserTemplateCommand
        {
            get { return _openusertemplatecommand; }
            set { Set(ref _openusertemplatecommand, value);}
        }

        private string _openusersettingscommand = "f9";
        public string OpenUserSettingsCommand 
        {
            get { return _openusersettingscommand; }
            set { Set(ref _openusersettingscommand, value);}
        }

        private string _togglefullscreencommand = "f11";
        public string ToggleFullScreenCommand 
        {
            get { return _togglefullscreencommand; }
            set { Set(ref _togglefullscreencommand, value);}
        }

        private string _togglepreviewcommand = "f12";
        public string TogglePreviousCommand
        {
            get { return _togglepreviewcommand; }
            set { Set(ref _togglepreviewcommand, value);}
        }

        private string _recentfilescommand = "ctrl+r";
        public string RecentFilesCommand
        {
            get { return _recentfilescommand; }
            set { Set(ref _recentfilescommand, value);}
        }

        private string _pastespecialcommand = "ctrl+shift+v";
        public string PasteSpecialCommand
        {
            get { return _pastespecialcommand; }
            set { Set(ref _pastespecialcommand, value);}
        }

        private string _showthemedialogcommand = "ctrl+t";
        public string ShowThemeDialogcommand
        {
            get { return _showthemedialogcommand; }
            set { Set(ref _showthemedialogcommand, value);}
        }

        private string _exporthtmlcommand = "ctrl+e";
        public string ExportHtmlCommand 
        {
            get { return _exporthtmlcommand; }
            set { Set(ref _exporthtmlcommand, value);}
        }

        private string _showgotolinedialogcommand = "ctrl+g";
        public string ShowGotoLineDialogCommand
        {
            get { return _showgotolinedialogcommand ; }
            set { Set(ref _showgotolinedialogcommand, value);}
        }

        private string _toggleautosavecommand = "alt+s";
        public string ToggleAutoSaveCommand
        {
            get { return _toggleautosavecommand; }
            set { Set(ref _toggleautosavecommand, value);}
        }

        private string _selectpreviousheadercommand = "ctrl+u";
        public string SelectPreviousHeaderCommand
        {
            get { return _selectpreviousheadercommand; }
            set { Set(ref _selectpreviousheadercommand, value);}
        }

        private string _selectnextheadercommand = "ctrl+j";
        public string SelectNextHeaderCommand
        {
            get { return _selectnextheadercommand; }
            set { Set(ref _selectnextheadercommand, value);}
        }

        private string _opennewinstancecommand = "ctrl+shift+n";
        public string OpenNewInstanceCommand 
        {
            get { return _opennewinstancecommand; }
            set { Set(ref _opennewinstancecommand, value);}
        }

        private string _insertfilecommand = "ctrl+shift+o";
        public string  InsertFileCommand 
        {
            get { return _insertfilecommand; }
            set { Set(ref _insertfilecommand, value);}
        }
        #endregion
    }
}
