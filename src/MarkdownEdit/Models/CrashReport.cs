using System;
using System.Text;
using Microsoft.Win32;

namespace MarkdownEdit.Models
{
    internal class CrashReport
    {
        private readonly string _divider = new string('-', 65);

        public CrashReport(Exception exception)
        {
            OperatingSystemInformation();
            Report = BuildReport(exception, OperatingSystemInformation());
        }

        public string Report { get; private set; }

        private string OperatingSystemInformation()
        {
            try
            {
                var osInfo = new StringBuilder();
                osInfo.AppendLine("Operation System Information");
                osInfo.AppendLine(_divider);
                osInfo.AppendLine(ProductName());
                osInfo.AppendLine("Service Pack: " + NativeMethods.GetServicePack());
                return osInfo.ToString();
            }
            catch (Exception)
            {
                return "Operating System Information unavailable";
            }
        }

        private static string ProductName()
        {
            try
            {
                const string subKey = @"SOFTWARE\Wow6432Node\Microsoft\Windows NT\CurrentVersion";
                var key = Registry.LocalMachine;
                var skey = key.OpenSubKey(subKey);
                return skey?.GetValue("ProductName")?.ToString() ?? string.Empty;
            }
            catch (Exception)
            {
                return "Product name unavailabe";
            }
        }

        private string BuildReport(Exception exception, string osInfo)
        {
            var report = new StringBuilder();
            report.AppendLine("MarkdownEdit Crash Report");
            report.AppendLine("Date: " + DateTime.UtcNow.ToString("u"));
            report.AppendLine("Version: " + Version.VersionNumber);
            report.AppendLine();
            report.AppendLine(_divider);
            report.AppendLine("*** Pressing Ctrl+C will copy the contents of this dialog ***");
            report.AppendLine("*** Email to: mike@mike-ward.net ***");
            report.AppendLine(_divider);
            report.AppendLine();
            report.AppendLine(osInfo);
            report.AppendLine();
            report.AppendLine("Exception");
            report.AppendLine(_divider);
            report.AppendLine(exception.ToString());
            return report.ToString();
        }
    }
}