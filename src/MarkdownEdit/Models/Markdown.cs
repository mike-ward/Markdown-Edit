using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using HtmlAgilityPack;
using MarkdownEdit.MarkdownConverters;
using MarkdownEdit.Properties;
using static System.String;

namespace MarkdownEdit.Models
{
    public static class Markdown
    {
        private static readonly IMarkdownConverter CommonMarkConverter = new CommonMarkConverter();
        private static readonly IMarkdownConverter GitHubMarkdownConverter = new GitHubMarkdownConverter();
        private static readonly IMarkdownConverter CustomMarkdownConverter = new CustomMarkdownConverter();

        public static string Wrap(string text) => Reformat(text);

        public static string WrapWithLinkReferences(string text) => Reformat(text, "--reference-links");

        public static string Unwrap(string text) => Reformat(text, "--no-wrap --atx-headers");

        public static string FromHtml(string path) => Pandoc(null, $"-f html -t {MarkdownFormat} --no-wrap \"{path}\"");

        public static string ToHtml(string markdown)
        {
            if (!IsNullOrWhiteSpace(App.UserSettings.CustomMarkdownConverter))
            {
                return CustomMarkdownConverter.ConvertToHtml(markdown);
            }
            return App.UserSettings.GitHubMarkdown
                ? GitHubMarkdownConverter.ConvertToHtml(markdown)
                : CommonMarkConverter.ConvertToHtml(markdown);
        }

        public static string FromMicrosoftWord(string path) => Pandoc(null, $"-f docx -t {MarkdownFormat} \"{path}\"");

        public static string ToMicrosoftWord(string markdown, string path) => 
            Pandoc(ResolveImageUrls(ToHtml(markdown)), $"-f html -t docx -o \"{path}\"");

        public static byte[] HtmlToPdf(string html) => 
            new NReco.PdfGenerator.HtmlToPdfConverter().GeneratePdf(ResolveImageUrls(html));

        public static string Pandoc(string text, string args)
        {
            var pandoc = PandocStartInfo(args, text != null);

            using (var process = Process.Start(pandoc))
            {
                if (process == null)
                {
                    MessageBox.Show("Error starting Pandoc", App.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return text;
                }
                if (text != null)
                {
                    var utf8 = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
                    utf8.Write(text);
                    utf8.Close();
                }
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0)
                {
                    var msg = process.StandardError.ReadToEnd();
                    result = string.IsNullOrWhiteSpace(msg) ? "empty error response" : msg;
                }
                return result;
            }
        }

        private static string MarkdownFormat => App.UserSettings.GitHubMarkdown
            ? "markdown_github"
            : "markdown_strict+fenced_code_blocks+backtick_code_blocks+intraword_underscores+strikeout";

        private static string Reformat(string text, string options = "")
        {
            var tuple = SeperateFrontMatter(text);
            var format = MarkdownFormat;
            var result = Pandoc(tuple.Item2, $"-f {format} -t {format} {options}");
            return tuple.Item1 + result;
        }

        private static ProcessStartInfo PandocStartInfo(string arguments, bool redirectInput)
        {
            return new ProcessStartInfo
            {
                FileName = "pandoc.exe",
                Arguments = arguments,
                RedirectStandardInput = redirectInput,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = Utility.AssemblyFolder()
            };
        }

        public static Tuple<string, string> SeperateFrontMatter(string text)
        {
            if (Regex.IsMatch(text, @"^---\s*$", RegexOptions.Multiline))
            {
                var matches = Regex.Matches(text, @"^(?:---)|(?:\.\.\.)\s*$", RegexOptions.Multiline);
                if (matches.Count < 2) return Tuple.Create(Empty, text);
                var match = matches[1];
                var index = match.Index + match.Groups[0].Value.Length + 1;
                while (index < text.Length && char.IsWhiteSpace(text[index])) index += 1;
                return Tuple.Create(text.Substring(0, index), text.Substring(index));
            }
            return Tuple.Create(Empty, text);
        }

        public static string SuggestFilenameFromTitle(string markdown)
        {
            var result = SeperateFrontMatter(markdown);
            if (IsNullOrEmpty(result.Item1)) return Empty;
            var pattern = new Regex(@"title:\s*(.+)", RegexOptions.Multiline);
            var match = pattern.Match(markdown);
            var title = match.Success ? match.Groups[1].Value : Empty;
            if (IsNullOrEmpty(title)) return Empty;
            var filename = DateTime.Now.ToString("yyyy-MM-dd-") + title.ToSlug(true);
            return filename;
        }

        private static string ResolveImageUrls(string html)
        {
            var filename = Settings.Default.LastOpenFile.StripOffsetFromFileName();
            if (IsNullOrWhiteSpace(filename)) return html;
            var folder = Path.GetDirectoryName(filename);
            if (IsNullOrWhiteSpace(folder)) return html;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            var images = doc.DocumentNode.SelectNodes("//img");
            if (images == null) return html;
            var modified = false;

            foreach (var image in images)
            {
                var src = image.GetAttributeValue("src", "");
                if (IsNullOrWhiteSpace(src)) continue;
                if (src.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) continue;
                var path = Path.Combine(folder, src);
                image.SetAttributeValue("src", path);
                modified = true;
            }

            return modified ? doc.DocumentNode.WriteTo() : html;
        }
    }
}