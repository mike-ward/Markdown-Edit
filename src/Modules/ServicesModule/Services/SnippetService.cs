using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using ICSharpCode.AvalonEdit.Snippets;
using Infrastructure;
using ServicesModule.Properties;

namespace ServicesModule.Services
{
    public class SnippetService : ISnippetService
    {
        private readonly INotify _notify;
        private static string SnippetFile { get; } = Path.Combine(Globals.UserSettingsFolder, "snippets.txt");

        private FileSystemWatcher _snippetFileWatcher;
        private Dictionary<string, string> _snippets;

        public SnippetService(INotify notify)
        {
            _notify = notify;
            _snippets = new Dictionary<string, string>();
        }

        public void Initialize()
        {
            ReadSnippetFile();
            if (_snippetFileWatcher == null)
            {
                _snippetFileWatcher = SnippetFile.WatchFile(ReadSnippetFile);
            }
        }

        public Snippet FindSnippet(string word)
        {
            return _snippets.TryGetValue(word, out var snippetText)
                ? BuildSnippet(snippetText)
                : null;
        }

        private async void ReadSnippetFile()
        {
            try
            {
                if (File.Exists(SnippetFile) == false)
                {
                    Directory.CreateDirectory(Globals.UserSettingsFolder);
                    File.WriteAllText(SnippetFile, Resources.Snippets);
                }

                var key = default(string);
                var value = default(string);
                _snippets = new Dictionary<string, string>();

                foreach (var line in File.ReadAllLines(SnippetFile))
                {
                    if (key != null)
                    {
                        // Keep reading until terminator sequence
                        if (line.TrimEnd() != "::")
                        {
                            value += line + Environment.NewLine;
                        }
                        else
                        {
                            _snippets.Add(key, value);
                            key = null;
                            value = null;
                        }
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var pair = line.Split(null, 2);

                    if (pair.Length == 2)
                    {
                        _snippets.Add(pair[0], pair[1]);
                    }

                    else if (pair[0].EndsWith("::") && pair[0][0] != ':')
                    {
                        // begin multiline sequence
                        key = pair[0].TrimEnd(':');
                        value = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                await _notify.Alert($"{ex.Message} in {SnippetFile}");
            }
        }

        private static Snippet BuildSnippet(string text)
        {
            var expanded = text
                .Trim()
                .Replace("\\r", "\r")
                .Replace("\\n", "\n")
                .Replace("\\t", "\t")
                .ReplaceDate();

            var snippet = new Snippet();
            var replaceable = new Regex(@"(\$\w+\$)");
            foreach (var token in replaceable.Split(expanded))
            {
                switch (token)
                {
                    case "$END$":
                        snippet.Elements.Add(new SnippetCaretElement());
                        break;
                    case "$CLIPBOARD$":
                        snippet.Elements.Add(new SnippetTextElement { Text = Clipboard.GetText() });
                        break;
                    default:
                        snippet.Elements.Add(replaceable.IsMatch(token)
                            ? new SnippetReplaceableTextElement { Text = token.Trim('$') }
                            : new SnippetTextElement { Text = token });
                        break;
                }
            }

            return snippet;
        }
    }
}
