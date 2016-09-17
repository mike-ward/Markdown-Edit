using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MarkdownEdit.MarkdownConverters;
using MarkdownEdit.Snippets;
using MarkdownEdit.SpellCheck;

namespace MarkdownEdit.Controls
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private IMarkdownConverter _commonMarkConverter;
        private string _titleName = string.Empty;
        private Thickness _editorMargins;
        private FindReplaceDialog _findReplaceDialog;
        private IMarkdownConverter _githubMarkdownConverter;
        private bool _newVersion;
        private ISnippetManager _snippetManager;
        private ISpellCheckProvider _spellCheckProvider;

        public string TitleName
        {
            get { return _titleName; }
            set { Set(ref _titleName, value); }
        }

        public Thickness EditorMargins
        {
            get { return _editorMargins; }
            set { Set(ref _editorMargins, value); }
        }

        public IMarkdownConverter CommonMarkConverter
        {
            get { return _commonMarkConverter; }
            set { Set(ref _commonMarkConverter, value); }
        }

        public IMarkdownConverter GitHubMarkdownConverter
        {
            get { return _githubMarkdownConverter; }
            set { Set(ref _githubMarkdownConverter, value); }
        }

        public ISpellCheckProvider SpellCheckProvider
        {
            get { return _spellCheckProvider; }
            set { Set(ref _spellCheckProvider, value); }
        }

        public FindReplaceDialog FindReplaceDialog
        {
            get { return _findReplaceDialog; }
            set { Set(ref _findReplaceDialog, value); }
        }

        public ISnippetManager SnippetManager
        {
            get { return _snippetManager; }
            set { Set(ref _snippetManager, value); }
        }

        public bool NewVersion
        {
            get { return _newVersion; }
            set { Set(ref _newVersion, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Set<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(property, value)) return;
            property = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
