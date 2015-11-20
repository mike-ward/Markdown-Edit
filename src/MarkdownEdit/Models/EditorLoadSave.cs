using System;
using System.IO;
using System.Linq;
using System.Windows;
using MarkdownEdit.Controls;
using MarkdownEdit.Properties;
using Microsoft.Win32;

namespace MarkdownEdit.Models
{
    public static class EditorLoadSave
    {
        public static void NewFile(Editor editor)
        {
            if (SaveIfModified(editor) == false) return;
            editor.Text = string.Empty;
            editor.IsModified = false;
            editor.FileName = string.Empty;
            Settings.Default.LastOpenFile = string.Empty;
        }

        public static bool LoadFile(Editor editor, string file, bool updateCursorPosition = true)
        {
            if (string.IsNullOrWhiteSpace(file)) return false;
            try
            {
                var parts = file.Split(new[] {'|'}, 2);
                var filename = parts[0] ?? "";
                var offset = ConvertToOffset(parts.Length == 2 ? parts[1] : "0");
                var pathExtension = Path.GetExtension(filename);
                var isWordDoc = pathExtension.Equals(".docx", StringComparison.OrdinalIgnoreCase);

                if (isWordDoc)
                {
                    NewFile(editor);
                    editor.EditBox.Text = ConvertText.FromMicrosoftWord(filename);
                    return true;
                }

                var isHtmlFile = pathExtension.Equals(".html", StringComparison.OrdinalIgnoreCase)
                    || pathExtension.Equals(".htm", StringComparison.OrdinalIgnoreCase);

                if (isHtmlFile)
                {
                    NewFile(editor);
                    editor.EditBox.Text = ConvertText.FromHtml(filename);
                    return true;
                }

                editor.EditBox.Text = File.ReadAllText(filename);

                if (updateCursorPosition)
                {
                    if (App.UserSettings.EditorOpenLastCursorPosition)
                    {
                        editor.EditBox.ScrollToLine(editor.EditBox.Document.GetLineByOffset(offset)?.LineNumber ?? 0);
                        editor.EditBox.SelectionStart = offset;
                    }
                    else
                    {
                        editor.EditBox.ScrollToHome();
                    }
                }

                Settings.Default.LastOpenFile = file;
                RecentFilesDialog.UpdateRecentFiles(filename, offset);
                editor.IsModified = false;
                editor.FileName = filename;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, App.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static bool SaveFile(Editor editor) => string.IsNullOrWhiteSpace(editor.FileName)
            ? SaveFileAs(editor)
            : Save(editor);

        public static bool SaveIfModified(Editor editor)
        {
            if (editor.IsModified == false) return true;

            var result = MessageBox.Show(
                @"Save your changes?",
                App.Title,
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            return (result == MessageBoxResult.Yes)
                ? SaveFile(editor)
                : result == MessageBoxResult.No;
        }

        public static bool SaveFileAs(Editor editor)
        {
            var filename = Utility.SaveFileDialog(
                Utility.SuggestFilenameFromTitle(editor.EditBox.Text),
                "Markdown files (*.md)|*.md|"
                    //+ "PDF files (*.pdf)|*.pdf|"
                    //+ "Word files (*.docx)|*.docx|"
                    + "All files (*.*)|*.*");

            if (string.IsNullOrWhiteSpace(filename)) return false;
            var currentFileName = editor.FileName;
            editor.FileName = filename;
            var offset = editor.EditBox.SelectionStart;
            if (!Save(editor) || !LoadFile(editor, filename, false))
            {
                editor.FileName = currentFileName;
                return false;
            }
            editor.EditBox.SelectionStart = offset;
            return true;
        }


        private static bool Save(Editor editor)
        {
            try
            {
                if (App.UserSettings.FormatOnSave) Editor.FormatCommand.Execute(true, editor);

                var lineEnd = "\r\n";
                if (App.UserSettings.LineEnding.Equals("cr", StringComparison.OrdinalIgnoreCase)) lineEnd = "\r";
                if (App.UserSettings.LineEnding.Equals("lf", StringComparison.OrdinalIgnoreCase)) lineEnd = "\n";

                var text = string.Join(
                    lineEnd,
                    editor.EditBox.Document.Lines.Select(line => editor.EditBox.Document.GetText(line).Trim('\r', '\n')));

                File.WriteAllText(editor.FileName, text);
                RecentFilesDialog.UpdateRecentFiles(editor.FileName, editor.EditBox.SelectionStart);
                Settings.Default.LastOpenFile = editor.FileName.AddOffsetToFileName(editor.EditBox.SelectionStart);
                editor.IsModified = false;
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, App.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        public static void OpenFile(Editor editor, string file)
        {
            if (SaveIfModified(editor) == false) return;
            if (string.IsNullOrWhiteSpace(file))
            {
                const string fileFilter =
                    "Markdown files (*.md)|*.md|" +
                        "Microsoft Word files (*.docx)|*.docx|" +
                        "HTML files (*.html)|*.html|" +
                        "All files (*.*)|*.*";

                var dialog = new OpenFileDialog {Filter = fileFilter};
                if (dialog.ShowDialog() == false) return;
                file = dialog.FileNames[0];
            }
            LoadFile(editor, file);
        }

        public static void InsertFile(Editor editor, string file)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(file))
                {
                    var dialog = new OpenFileDialog();
                    var result = dialog.ShowDialog();
                    if (result == false) return;
                    file = dialog.FileNames[0];
                }
                var text = File.ReadAllText(file);
                editor.EditBox.Document.Insert(editor.EditBox.SelectionStart, text);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, App.Title, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static int ConvertToOffset(string number)
        {
            int offset;
            return (int.TryParse(number, out offset)) ? offset : 0;
        }
    }
}