using System.Diagnostics;
using System.IO;
using System.Text;
using MarkdownEdit.Models;

namespace MarkdownEdit.MarkdownConverters
{
    public class CustomMarkdownConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown)
        {
            var converter = App.UserSettings.CustomMarkdownConverter;
            var converterArgs = App.UserSettings.CustomMarkdownConverterArgs;
            return RunConverter(converter, converterArgs, markdown);
        }

        private static string RunConverter(string converter, string convertArguments, string text)
        {
            var args = convertArguments ?? "";
            var program = StartInfo(converter, args, text != null);

            using (var process = Process.Start(program))
            {
                if (process == null)
                {
                    Notify.Alert($"Error starting {converter}");
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

        private static ProcessStartInfo StartInfo(string fileName, string arguments, bool redirectInput)
        {
            return new ProcessStartInfo
            {
                FileName = fileName,
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