using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Environment;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Input;

namespace MarkdownEdit
{
    public class InputBindingSettings
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
        /// Load key binding settings from file
        /// </summary>
        /// <returns></returns>
        private static InputBinding Load()
        {
            // create default binding file
            if (!File.Exists(KeyBindingFile))
            {
                var binding = new InputBinding();
                Save(binding);
                return binding;
            }

            var bindingFromFile= JsonConvert.DeserializeObject<InputBinding>(File.ReadAllText(KeyBindingFile));
            return bindingFromFile;
        }

        /// <summary>
        /// Only called for create fresh default binding
        /// </summary>
        /// <param name="binding"></param>
        private static void Save(InputBinding binding)
        {
            File.WriteAllText(KeyBindingFile , JsonConvert.SerializeObject(binding, Formatting.Indented));
        }

        public static void Update()
        {
            if (_window == null) return;

            var mergedBinding = new InputBinding();
            mergedBinding.Merge(InputBindingSettings.Load());

            _window.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, (Action)delegate(){
                var _collection = _window.InputBindings;
                _collection.Clear();
                _collection.Add(new KeyBinding(EditingCommands.ToggleBold, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ToggleBold)));
                _collection.Add(new KeyBinding(EditingCommands.ToggleItalic, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ToggleItalic)));
                _collection.Add(new KeyBinding(MainWindow.ToggleCodeCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ToggleCode)));
                _collection.Add(new KeyBinding(ApplicationCommands.SaveAs, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.SaveAs)));
                _collection.Add(new KeyBinding(MainWindow.ToggleWordWrapCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ToggleWordWrap)));
                _collection.Add(new KeyBinding(MainWindow.FindNextCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.FindNext)));
                _collection.Add(new KeyBinding(MainWindow.FindPreviousCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.FindPrevious)));

                var insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.InsertHeader1));
                insertHeaderBinding.CommandParameter = "1";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.InsertHeader2));
                insertHeaderBinding.CommandParameter = "2";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.InsertHeader3));
                insertHeaderBinding.CommandParameter = "3";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.InsertHeader4));
                insertHeaderBinding.CommandParameter = "4";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.InsertHeader5));
                insertHeaderBinding.CommandParameter = "5";
                _collection.Add(insertHeaderBinding);

                insertHeaderBinding = new KeyBinding(MainWindow.InsertHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.InsertHeader6));
                insertHeaderBinding.CommandParameter = "6";
                _collection.Add(insertHeaderBinding);

                _collection.Add(new KeyBinding(EditingCommands.IncreaseFontSize, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.IncreaseFontSize)));
                _collection.Add(new KeyBinding(EditingCommands.DecreaseFontSize, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.DecreaseFontSize)));
                _collection.Add(new KeyBinding(MainWindow.RestoreFontSizeCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.RestoreFontSize)));
                _collection.Add(new KeyBinding(MainWindow.OpenUserSnippetsCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.OpenUserSnippets)));
                _collection.Add(new KeyBinding(MainWindow.OpenUserDictionaryCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.OpenUserDictionary)));
                _collection.Add(new KeyBinding(MainWindow.ToggleSpellCheckCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ToggleSpellCheck)));
                _collection.Add(new KeyBinding(MainWindow.OpenUserTemplateCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.OpenUserTemplate)));
                _collection.Add(new KeyBinding(MainWindow.OpenUserSettingsCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.OpenUserSettings)));
                _collection.Add(new KeyBinding(MainWindow.ToggleFullScreenCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ToggleFullScreen)));
                _collection.Add(new KeyBinding(MainWindow.TogglePreviewCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.TogglePrevious)));
                _collection.Add(new KeyBinding(MainWindow.RecentFilesCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.RecentFiles)));
                _collection.Add(new KeyBinding(MainWindow.PasteSpecialCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.PasteSpecial)));
                _collection.Add(new KeyBinding(MainWindow.ShowThemeDialogCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ShowThemeDialog)));
                _collection.Add(new KeyBinding(MainWindow.ExportHtmlCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ExportHtml)));
                _collection.Add(new KeyBinding(MainWindow.ShowGotoLineDialogCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ShowGotoLineDialog)));
                _collection.Add(new KeyBinding(MainWindow.ToggleAutoSaveCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.ToggleAutoSave)));
                _collection.Add(new KeyBinding(MainWindow.SelectPreviousHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.SelectPreviousHeader)));
                _collection.Add(new KeyBinding(MainWindow.SelectNextHeaderCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.SelectNextHeader)));
                _collection.Add(new KeyBinding(MainWindow.OpenNewInstanceCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.OpenNewInstance)));
                _collection.Add(new KeyBinding(MainWindow.InsertFileCommand, (KeyGesture)new KeyGestureConverter().ConvertFromString(mergedBinding.InsertFile)));
            });
        }
    }

    public class InputBinding : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value) || value == null) return;
            property = value;
            OnPropertyChanged(propertyName);
        }

        public void Merge(InputBinding update)
        {
            PropertyInfo[] destinationProperties = this.GetType().GetProperties();
            foreach (PropertyInfo destinationPI in destinationProperties)
            {
                PropertyInfo sourcePI = update.GetType().GetProperty(destinationPI.Name);
                destinationPI.SetValue(this, sourcePI.GetValue(update, null), null);
            }
        }

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
        public string ToggleCode
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
        public string ToggleWordWrap
        {
            get { return _togglewordwrapcommand; }
            set { Set(ref _togglewordwrapcommand, value);}
        }

        private string _findnextcommand = "f3";
        public string FindNext 
        {
            get { return _findnextcommand; }
            set { Set(ref _findnextcommand, value);}
        }

        private string _findpreviouscommand = "shift+f3";
        public string FindPrevious 
        {
            get { return _findpreviouscommand; }
            set { Set(ref _findpreviouscommand, value);}
        }

        private string _insertheadercommand1 = "ctrl+1";
        public string InsertHeader1
        {
            get { return _insertheadercommand1; }
            set { Set(ref _insertheadercommand1, value);}
        }

        private string _insertheadercommand2 = "ctrl+2";
        public string InsertHeader2
        {
            get { return _insertheadercommand2; }
            set { Set(ref _insertheadercommand2, value);}
        }
        
        private string _insertheadercommand3 = "ctrl+3";
        public string InsertHeader3
        {
            get { return _insertheadercommand3; }
            set { Set(ref _insertheadercommand3, value);}
        }

        private string _insertheadercommand4 = "ctrl+4";
        public string InsertHeader4
        {
            get { return _insertheadercommand4; }
            set { Set(ref _insertheadercommand4, value);}
        }

        private string _insertheadercommand5 = "ctrl+5";
        public string InsertHeader5
        {
            get { return _insertheadercommand5; }
            set { Set(ref _insertheadercommand5, value);}
        }

        private string _insertheadercommand6 = "ctrl+6";
        public string InsertHeader6
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
        public string RestoreFontSize
        {
            get { return _restorefontsizecommand; }
            set { Set(ref _restorefontsizecommand, value);}
        }

        private string _openusersnippetscommand = "f6";
        public string OpenUserSnippets 
        {
            get { return _openusersnippetscommand ; }
            set { Set(ref _openusersnippetscommand, value);}
        }

        private string _openuserdictionarycommand = "f7";
        public string OpenUserDictionary
        {
            get { return _openuserdictionarycommand; }
            set { Set(ref _openuserdictionarycommand, value);}
        }

        private string _togglespellcheckcommand = "ctrl+f7";
        public string ToggleSpellCheck 
        {
            get { return _togglespellcheckcommand; }
            set { Set(ref _togglespellcheckcommand, value);}
        }

        private string _openusertemplatecommand = "f8";
        public string OpenUserTemplate
        {
            get { return _openusertemplatecommand; }
            set { Set(ref _openusertemplatecommand, value);}
        }

        private string _openusersettingscommand = "f9";
        public string OpenUserSettings 
        {
            get { return _openusersettingscommand; }
            set { Set(ref _openusersettingscommand, value);}
        }

        private string _togglefullscreencommand = "f11";
        public string ToggleFullScreen 
        {
            get { return _togglefullscreencommand; }
            set { Set(ref _togglefullscreencommand, value);}
        }

        private string _togglepreviewcommand = "f12";
        public string TogglePrevious
        {
            get { return _togglepreviewcommand; }
            set { Set(ref _togglepreviewcommand, value);}
        }

        private string _recentfilescommand = "ctrl+r";
        public string RecentFiles
        {
            get { return _recentfilescommand; }
            set { Set(ref _recentfilescommand, value);}
        }

        private string _pastespecialcommand = "ctrl+shift+v";
        public string PasteSpecial
        {
            get { return _pastespecialcommand; }
            set { Set(ref _pastespecialcommand, value);}
        }

        private string _showthemedialogcommand = "ctrl+t";
        public string ShowThemeDialog
        {
            get { return _showthemedialogcommand; }
            set { Set(ref _showthemedialogcommand, value);}
        }

        private string _exporthtmlcommand = "ctrl+e";
        public string ExportHtml 
        {
            get { return _exporthtmlcommand; }
            set { Set(ref _exporthtmlcommand, value);}
        }

        private string _showgotolinedialogcommand = "ctrl+g";
        public string ShowGotoLineDialog
        {
            get { return _showgotolinedialogcommand ; }
            set { Set(ref _showgotolinedialogcommand, value);}
        }

        private string _toggleautosavecommand = "alt+s";
        public string ToggleAutoSave
        {
            get { return _toggleautosavecommand; }
            set { Set(ref _toggleautosavecommand, value);}
        }

        private string _selectpreviousheadercommand = "ctrl+u";
        public string SelectPreviousHeader
        {
            get { return _selectpreviousheadercommand; }
            set { Set(ref _selectpreviousheadercommand, value);}
        }

        private string _selectnextheadercommand = "ctrl+j";
        public string SelectNextHeader
        {
            get { return _selectnextheadercommand; }
            set { Set(ref _selectnextheadercommand, value);}
        }

        private string _opennewinstancecommand = "ctrl+shift+n";
        public string OpenNewInstance
        {
            get { return _opennewinstancecommand; }
            set { Set(ref _opennewinstancecommand, value);}
        }

        private string _insertfilecommand = "ctrl+shift+o";
        public string  InsertFile 
        {
            get { return _insertfilecommand; }
            set { Set(ref _insertfilecommand, value);}
        }
    }
}
