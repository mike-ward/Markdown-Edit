using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;

namespace MarkdownEdit.Models
{
    internal static class ConvertText
    {
        private const string CommonMarkArgs = "markdown_strict+fenced_code_blocks+backtick_code_blocks+intraword_underscores\x20";
        private const string CommonMark = "-f " + CommonMarkArgs + "-t " + CommonMarkArgs;

        public static string Wrap(string text) => RunPandoc(text, "--columns 80");

        public static string WrapWithLinkReferences(string text) => RunPandoc(text, "--columns 80 --reference-links");

        public static string Unwrap(string text) => RunPandoc(text, "--no-wrap --atx-headers");

        private static string RunPandoc(string text, string options)
        {
            var tuple = Utility.SeperateFrontMatter(text);
            var result = Pandoc(tuple.Item2, CommonMark + options);
            return tuple.Item1 + result;
        }

        public static string FromMicrosoftWord(string path)
        {
            var args = $"-f docx -t {CommonMarkArgs} \"{path}\"";
            var info = PandocInfo(args);
            info.RedirectStandardInput = false;

            using (var process = Process.Start(info))
            {
                if (process == null)
                {
                    MessageBox.Show("Error starting Pandoc", App.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return string.Empty;
                }
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0) result = process.StandardError.ReadToEnd();
                return result;
            }
        }

        private static string Pandoc(string text, string args)
        {
            var info = PandocInfo(args);

            using (var process = Process.Start(info))
            {
                if (process == null)
                {
                    MessageBox.Show("Error starting Pandoc", App.Title, MessageBoxButton.OK, MessageBoxImage.Error);
                    return text;
                }
                var utf8 = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
                utf8.Write(text);
                utf8.Close();
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0) result = process.StandardError.ReadToEnd();
                return result;
            }
        }

        private static ProcessStartInfo PandocInfo(string arguments)
        {
            return new ProcessStartInfo
            {
                FileName = "pandoc.exe",
                Arguments = arguments,
                RedirectStandardInput = true,
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