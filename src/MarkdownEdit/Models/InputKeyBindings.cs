using System;
using System.Windows.Input;

namespace MarkdownEdit
{
    public class InputKeyBindings
    {
        private static void Set(ref string property, string value)
        {
            if (string.IsNullOrWhiteSpace(value) || property.Equals(value, StringComparison.OrdinalIgnoreCase)) return;
            try
            {
                if (new KeyGestureConverter().ConvertFromString(value) != null)
                {
                    property = value;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void Merge(InputKeyBindings updates)
        {
            foreach (var property in GetType().GetProperties())
            {
                var updatedProperty = updates.GetType().GetProperty(property.Name);
                if (updatedProperty != null)
                {
                    property.SetValue(this, updatedProperty.GetValue(updates, null), null);
                }
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
            set { Set(ref _togglewordwrapcommand, value); }
        }

        private string _findnextcommand = "f3";

        public string FindNext
        {
            get { return _findnextcommand; }
            set { Set(ref _findnextcommand, value); }
        }

        private string _findpreviouscommand = "shift+f3";

        public string FindPrevious
        {
            get { return _findpreviouscommand; }
            set { Set(ref _findpreviouscommand, value); }
        }

        private string _insertheadercommand1 = "ctrl+1";

        public string InsertHeader1
        {
            get { return _insertheadercommand1; }
            set { Set(ref _insertheadercommand1, value); }
        }

        private string _insertheadercommand2 = "ctrl+2";

        public string InsertHeader2
        {
            get { return _insertheadercommand2; }
            set { Set(ref _insertheadercommand2, value); }
        }

        private string _insertheadercommand3 = "ctrl+3";

        public string InsertHeader3
        {
            get { return _insertheadercommand3; }
            set { Set(ref _insertheadercommand3, value); }
        }

        private string _insertheadercommand4 = "ctrl+4";

        public string InsertHeader4
        {
            get { return _insertheadercommand4; }
            set { Set(ref _insertheadercommand4, value); }
        }

        private string _insertheadercommand5 = "ctrl+5";

        public string InsertHeader5
        {
            get { return _insertheadercommand5; }
            set { Set(ref _insertheadercommand5, value); }
        }

        private string _insertheadercommand6 = "ctrl+6";

        public string InsertHeader6
        {
            get { return _insertheadercommand6; }
            set { Set(ref _insertheadercommand6, value); }
        }

        private string _increasefontsize = "ctrl+plus";

        public string IncreaseFontSize
        {
            get { return _increasefontsize; }
            set { Set(ref _increasefontsize, value); }
        }

        private string _decreasefontsize = "ctrl+minus";

        public string DecreaseFontSize
        {
            get { return _decreasefontsize; }
            set { Set(ref _decreasefontsize, value); }
        }

        private string _restorefontsizecommand = "ctrl+0";

        public string RestoreFontSize
        {
            get { return _restorefontsizecommand; }
            set { Set(ref _restorefontsizecommand, value); }
        }

        private string _openusersnippetscommand = "f6";

        public string OpenUserSnippets
        {
            get { return _openusersnippetscommand; }
            set { Set(ref _openusersnippetscommand, value); }
        }

        private string _openuserdictionarycommand = "f7";

        public string OpenUserDictionary
        {
            get { return _openuserdictionarycommand; }
            set { Set(ref _openuserdictionarycommand, value); }
        }

        private string _togglespellcheckcommand = "ctrl+f7";

        public string ToggleSpellCheck
        {
            get { return _togglespellcheckcommand; }
            set { Set(ref _togglespellcheckcommand, value); }
        }

        private string _openusertemplatecommand = "f8";

        public string OpenUserTemplate
        {
            get { return _openusertemplatecommand; }
            set { Set(ref _openusertemplatecommand, value); }
        }

        private string _openusersettingscommand = "f9";

        public string OpenUserSettings
        {
            get { return _openusersettingscommand; }
            set { Set(ref _openusersettingscommand, value); }
        }

        private string _openkeybindingsettingscommand = "f10";

        public string OpenKeyBindingSettings
        {
            get { return _openkeybindingsettingscommand; }
            set { Set(ref _openkeybindingsettingscommand, value); }
        }

        private string _togglefullscreencommand = "f11";

        public string ToggleFullScreen
        {
            get { return _togglefullscreencommand; }
            set { Set(ref _togglefullscreencommand, value); }
        }

        private string _togglepreviewcommand = "f12";

        public string TogglePreview
        {
            get { return _togglepreviewcommand; }
            set { Set(ref _togglepreviewcommand, value); }
        }

        private string _recentfilescommand = "ctrl+r";

        public string RecentFiles
        {
            get { return _recentfilescommand; }
            set { Set(ref _recentfilescommand, value); }
        }

        private string _pastespecialcommand = "ctrl+shift+v";

        public string PasteSpecial
        {
            get { return _pastespecialcommand; }
            set { Set(ref _pastespecialcommand, value); }
        }

        private string _showthemedialogcommand = "ctrl+t";

        public string ShowThemeDialog
        {
            get { return _showthemedialogcommand; }
            set { Set(ref _showthemedialogcommand, value); }
        }

        private string _exporthtmlcommand = "ctrl+e";

        public string ExportHtml
        {
            get { return _exporthtmlcommand; }
            set { Set(ref _exporthtmlcommand, value); }
        }

        private string _showgotolinedialogcommand = "ctrl+g";

        public string ShowGotoLineDialog
        {
            get { return _showgotolinedialogcommand; }
            set { Set(ref _showgotolinedialogcommand, value); }
        }

        private string _toggleautosavecommand = "alt+s";

        public string ToggleAutoSave
        {
            get { return _toggleautosavecommand; }
            set { Set(ref _toggleautosavecommand, value); }
        }

        private string _selectpreviousheadercommand = "ctrl+u";

        public string SelectPreviousHeader
        {
            get { return _selectpreviousheadercommand; }
            set { Set(ref _selectpreviousheadercommand, value); }
        }

        private string _selectnextheadercommand = "ctrl+j";

        public string SelectNextHeader
        {
            get { return _selectnextheadercommand; }
            set { Set(ref _selectnextheadercommand, value); }
        }

        private string _opennewinstancecommand = "ctrl+shift+n";

        public string OpenNewInstance
        {
            get { return _opennewinstancecommand; }
            set { Set(ref _opennewinstancecommand, value); }
        }

        private string _insertfilecommand = "ctrl+shift+o";

        public string InsertFile
        {
            get { return _insertfilecommand; }
            set { Set(ref _insertfilecommand, value); }
        }
    }
}