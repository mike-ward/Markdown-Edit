using System.Diagnostics;
using System.IO;
using System.Text;

namespace MarkdownEdit.Models
{
    internal static class ConvertText
    {
        private const string CommonMarkArgs = "commonmark\x20";
        private const string CommonMark = "-f " + CommonMarkArgs + "-t " + CommonMarkArgs;

        public static string Wrap(string text)
        {
            var tuple = Utility.SeperateFrontMatter(text);
            var result = Pandoc(tuple.Item2, CommonMark + "--columns 80");
            return tuple.Item1 + result;
        }

        public static string Unwrap(string text)
        {
            var tuple = Utility.SeperateFrontMatter(text);
            var result = Pandoc(tuple.Item2, CommonMark + "--no-wrap --atx-headers");
            return tuple.Item1 + result;
        }

        public static string FromMicrosoftWord(string path)
        {
            var args = $"-f docx -t {CommonMarkArgs} \"{path}\"";
            var info = PandocInfo(args);
            info.RedirectStandardInput = false;

            using (var process = Process.Start(info))
            {
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