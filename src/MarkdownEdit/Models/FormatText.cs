using System.Diagnostics;
using System.IO;
using System.Text;

namespace MarkdownEdit.Models
{
    internal static class FormatText
    {
        public static string Prettify(string text)
        {
            var tuple = Utility.SeperateFrontMatter(text);
            var result = Pandoc(tuple.Item2, "--columns 80");
            return tuple.Item1 + result;
        }

        public static string Uglify(string text)
        {
            var tuple = Utility.SeperateFrontMatter(text);
            var result = Pandoc(tuple.Item2, "--no-wrap --atx-headers");
            return tuple.Item1 + result;
        }

        private static string Pandoc(string text, string args)
        {
            var info = PandocInfo();
            info.Arguments += $" {args}";

            using (var process = Process.Start(info))
            {
                var utf8 = new StreamWriter(process.StandardInput.BaseStream, Encoding.UTF8);
                utf8.Write(text);
                utf8.Close();
                var result = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                if (process.ExitCode != 0) result = process.StandardError.ReadToEnd();
                return result;
            }
        }

        private static ProcessStartInfo PandocInfo()
        {
            return new ProcessStartInfo
            {
                FileName = "pandoc.exe",
                Arguments = "-f markdown -t markdown",
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