using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using MarkdownEdit.Models;

namespace MarkdownEdit.MarkdownConverters
{
    public class CustomMarkdownConverter : IMarkdownConverter
    {
        public string ConvertToHtml(string markdown)
        {
            var converter = App.UserSettings.CustomMarkdownConverter;
            return RunConverter(converter, markdown);
        }

        private static string RunConverter(string converter, string text)
        {
            var separator = converter.StartsWith("\"") ? new[] {'"'} : null;
            var split = converter.Split(separator, 2);
            var fileName = split[0];
            var args = split[1];
            var program = StartInfo(fileName, args, text != null);

            using (var process = Process.Start(program))
            {
                if (process == null)
                {
                    MessageBox.Show($"Error starting {fileName}", App.Title, MessageBoxButton.OK, MessageBoxImage.Error);
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