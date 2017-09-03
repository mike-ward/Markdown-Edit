using System;
using ICSharpCode.AvalonEdit;
using Infrastructure;
using Prism.Mvvm;

namespace EditModule.ViewModels
{
    public class FindReplaceDialogViewModel : BindableBase
    {
        private readonly IEditService _editService;
        private string _findText;
        private bool _caseSensitive;
        private bool _wholeWord;
        private bool _useRegex;
        private bool _wildcards;
        private bool _searchUp;

        // ReSharper disable once InconsistentNaming
        private static readonly FindReplaceOptions _findReplaceOptions = new FindReplaceOptions();

        public FindReplaceDialogViewModel(IEditService editService)
        {
            _editService = editService;
        }

        public void OnLoad(object sender, EventArgs ea)
        {
            LoadDialogFields();
        }

        private  void LoadDialogFields()
        {
            FindText = _findReplaceOptions.FindText;
            CaseInsensitive = _findReplaceOptions.CaseSensitive;
            WholeWord = _findReplaceOptions.WholeWord;
            UseRegEx = _findReplaceOptions.Regex;
            Wildcards = _findReplaceOptions.Wildcards;
            SearchUp = _findReplaceOptions.SearchUp;
        }

        private void UpdateFindReplaceOptions()
        {
            _findReplaceOptions.FindText = FindText;
            _findReplaceOptions.CaseSensitive = CaseInsensitive;
            _findReplaceOptions.WholeWord = WholeWord;
            _findReplaceOptions.Regex = UseRegEx;
            _findReplaceOptions.Wildcards = Wildcards;
            _findReplaceOptions.SearchUp = SearchUp;
        }

        public void OnClose(object sender, EventArgs ea)
        {
            UpdateFindReplaceOptions();
        }

        public bool FindNext(TextEditor textEditor)
        {
            UpdateFindReplaceOptions();
            return _editService.FindNext(textEditor, _findReplaceOptions);
        }

        public void Replace(TextEditor textEditor)
        {
            UpdateFindReplaceOptions();
            _editService.Replace(textEditor, _findReplaceOptions);
        }

        public void ReplaceAll(TextEditor textEditor)
        {
            UpdateFindReplaceOptions();
            _editService.ReplaceAll(textEditor, _findReplaceOptions);
        }

        // Properties

        public string FindText
        {
            get => _findText;
            set => SetProperty(ref _findText, value);
        }

        public bool CaseInsensitive
        {
            get => _caseSensitive;
            set => SetProperty(ref _caseSensitive, value);
        }

        public bool WholeWord
        {
            get => _wholeWord;
            set => SetProperty(ref _wholeWord, value);
        }

        public bool UseRegEx
        {
            get => _useRegex;
            set => SetProperty(ref _useRegex, value);
        }

        public bool Wildcards
        {
            get => _wildcards;
            set => SetProperty(ref _wildcards, value);
        }

        public bool SearchUp
        {
            get => _searchUp;
            set => SetProperty(ref _searchUp, value);
        }
    }
}
