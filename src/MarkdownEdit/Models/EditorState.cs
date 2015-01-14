using System;
using MarkdownEdit.Controls;

namespace MarkdownEdit.Models
{
    internal struct EditorState
    {
        public bool StateSaved { get; private set; }
        private string _text;
        private bool _isModified;
        private bool _wordWrap;
        private bool _spellCheck;
        private int _caretOffset;
        private double _verticalOffset;

        public void Save(Editor editor)
        {
            _text = editor.Text;
            _isModified = editor.IsModified;
            _wordWrap = editor.WordWrap;
            _spellCheck = editor.SpellCheck;
            _verticalOffset = editor.EditBox.VerticalOffset;
            _caretOffset = editor.EditBox.CaretOffset;
            editor.IsModified = false;
            editor.WordWrap = true;
            editor.IsReadOnly = true;
            editor.SpellCheck = false;
            editor.EditBox.ScrollToHome();
            StateSaved = true;
        }

        public void Restore(Editor editor)
        {
            if (StateSaved == false) return;
            editor.Text = _text;
            editor.IsModified = _isModified;
            editor.WordWrap = _wordWrap;
            editor.IsReadOnly = false;
            editor.SpellCheck = _spellCheck;
            editor.DisplayName = String.Empty;
            editor.EditBox.ScrollToVerticalOffset(_verticalOffset);
            editor.EditBox.CaretOffset = _caretOffset;
            StateSaved = false;
            editor.Dispatcher.Invoke(() => editor.EditBox.Focus());
        }
    }
}