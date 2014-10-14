using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using MarkdownEdit.Properties;

namespace MarkdownEdit
{
    public partial class Editor
    {
        private string TextSwap { get; set; }

        public Editor()
        {
            InitializeComponent();
            EditorBox.Loaded += EditorBoxOnLoaded;
        }

        private void EditorBoxOnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            LoadFile(Settings.Default.LastOpenFile);
            EditorBox.TextChanged += EditorBoxOnTextChanged;
            EditorBox.Dispatcher.InvokeAsync(() => EditorBoxOnTextChanged(this, null));

            var reader = new XmlTextReader(new StringReader(Properties.Resources.markdown_xshd));
            var xshd = HighlightingLoader.LoadXshd(reader);
            var highlighter = HighlightingLoader.Load(xshd, HighlightingManager.Instance);
            EditorBox.SyntaxHighlighting = highlighter;
            reader.Close();

            EditorBox.Options.IndentationSize = 2;
            EditorBox.Options.ConvertTabsToSpaces = true;
            EditorBox.Options.AllowScrollBelowDocument = true;
            EditorBox.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
        }

        private void EditorBoxOnTextChanged(object sender, EventArgs eventArgs)
        {
            MainWindow.UpdatePreviewCommand.Execute(EditorBox.Text, this);
        }

        public void OpenFileHandler()
        {
            var dialog = new OpenFileDialog();
            var result = dialog.ShowDialog();
            if (result != DialogResult.OK) return;
            LoadFile(dialog.FileNames[0]);
        }

        private void LoadFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file)) return;
            EditorBox.Text = File.ReadAllText(file);
            FileLoaded(file);
        }

        private void FileLoaded(string file)
        {
            Settings.Default.LastOpenFile = file;
            MainWindow.SetTitleFileNameCommand.Execute(Path.GetFileName(file), this);            
        }

        public void ToggleHelp()
        {
            if (TextSwap == null)
            {
                TextSwap = EditorBox.Text;
                EditorBox.Text = Properties.Resources.Help;
                EditorBox.IsReadOnly = true;
            }
            else
            {
                EditorBox.IsReadOnly = false;
                EditorBox.Text = TextSwap;
                TextSwap = null;
            }
        }

        public void WordWrapHandler()
        {
            EditorBox.WordWrap = !EditorBox.WordWrap;
        }

        private void ScrollViewerOnScrollChanged(object sender, ScrollChangedEventArgs scrollChangedEventArgs)
        {
            MainWindow.ScrollPreviewCommand.Execute(EditorBox.VerticalOffset, this);
        }
    }
}