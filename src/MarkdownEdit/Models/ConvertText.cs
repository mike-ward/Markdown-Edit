using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace MarkdownEdit.Models
{
    internal static class ConvertText
    {
        public static string Wrap(string text) => Reformat(text);

        public static string WrapWithLinkReferences(string text) => Reformat(text, "--reference-links");

        public static string Unwrap(string text) => Reformat(text, "--no-wrap --atx-headers");

        public static string FromHtml(string path) => Pandoc(null, $"-f html -t {MarkdownFormat} --no-wrap \"{path}\"");

        public static string FromMicrosoftWord(string path) => Pandoc(null, $"-f docx -t {MarkdownFormat} \"{path}\"");

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
                if (process.ExitCode != 0) result = process.StandardError.ReadToEnd();
                return result;
            }
        }

        private static string MarkdownFormat => App.UserSettings.GitHubMarkdown
            ? "markdown_github"
            : "markdown_strict+fenced_code_blocks+backtick_code_blocks+intraword_underscores+strikeout";

        private static string Reformat(string text, string options = "")
        {
            var tuple = Utility.SeperateFrontMatter(text);
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
    }
}