using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MarkdownEdit.Models
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);

        [StructLayout(LayoutKind.Sequential)]
        internal struct OsVersioninfo
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string szCSDVersion;
        }

        [DllImport("kernel32.Dll")]
        internal static extern short GetVersionEx(ref OsVersioninfo o);

        public static string GetServicePack()
        {
            try
            {
                var os = new OsVersioninfo { dwOSVersionInfoSize = Marshal.SizeOf(typeof(OsVersioninfo)) };
                GetVersionEx(ref os);
                return (os.szCSDVersion == "") ? "No Service Pack detected" : os.szCSDVersion;
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.Message);
                return "Service pack version unavailable";
            }
        }
    }
}