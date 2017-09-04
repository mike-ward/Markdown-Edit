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
        private string _replaceText;
        private bool _caseSensitive;
        private bool _wholeWord;
        private bool _useRegex;
        private bool _wildcards;

        // ReSharper disable once InconsistentNaming

        public FindReplaceDialogViewModel(IEditService editService)
        {
            _editService = editService;
        }

        public void OnLoad(object sender, EventArgs ea)
        {
            FindText = _editService.FindReplaceOptions.FindText;
            CaseSensitive = _editService.FindReplaceOptions.CaseSensitive;
            WholeWord = _editService.FindReplaceOptions.WholeWord;
            UseRegEx = _editService.FindReplaceOptions.Regex;
            Wildcards = _editService.FindReplaceOptions.Wildcards;
        }

        private void UpdateFindReplaceOptions()
        {
            _editService.FindReplaceOptions.FindText = FindText;
            _editService.FindReplaceOptions.CaseSensitive = CaseSensitive;
            _editService.FindReplaceOptions.WholeWord = WholeWord;
            _editService.FindReplaceOptions.Regex = UseRegEx;
            _editService.FindReplaceOptions.Wildcards = Wildcards;
        }

        public void OnClose(object sender, EventArgs ea)
        {
            UpdateFindReplaceOptions();
        }

        public bool FindNext(TextEditor textEditor)
        {
            UpdateFindReplaceOptions();
            return _editService.FindNext(textEditor, _editService.FindReplaceOptions);
        }

        public void Replace(TextEditor textEditor)
        {
            UpdateFindReplaceOptions();
            _editService.Replace(textEditor, _editService.FindReplaceOptions);
        }

        public void ReplaceAll(TextEditor textEditor)
        {
            UpdateFindReplaceOptions();
            _editService.ReplaceAll(textEditor, _editService.FindReplaceOptions);
        }

        // Properties

        public string FindText
        {
            get => _findText;
            set => SetProperty(ref _findText, value);
        }

        public string ReplaceText
        {
            get => _replaceText;
            set => SetProperty(ref _replaceText, value);
        }

        public bool CaseSensitive
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
    }
}
