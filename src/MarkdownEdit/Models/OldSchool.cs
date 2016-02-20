using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace MarkdownEdit.Models
{
    internal static class OldSchool
    {
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int uMsg, IntPtr wparam, IntPtr lparam);

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public IntPtr cbData;
            public IntPtr lpData;
        }

        public static bool IsEditingFile(Process process, string path)
        {
            var result = Send(path, process.MainWindowHandle);
            return result == 1;
        }

        public static int Send(string data, IntPtr windowHandle)
        {
            var cds = new COPYDATASTRUCT();
            cds.dwData = (IntPtr)Marshal.SizeOf(cds);
            cds.cbData = (IntPtr)data.Length;
            cds.lpData = Marshal.StringToHGlobalAnsi(data);

            var ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(cds));
            Marshal.StructureToPtr(cds, ptr, true);

            try
            {
                const int WM_COPY_DATA = 0x004A;
                var result = SendMessage(windowHandle, WM_COPY_DATA, IntPtr.Zero, ptr);
                return result;
            }
            catch (Exception)
            {
                return 1;
            }
            finally
            {
                Marshal.FreeHGlobal(cds.lpData);
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        public static string CopyDataStructToString(IntPtr data)
        {
            var cps = (COPYDATASTRUCT)Marshal.PtrToStructure(data, typeof(COPYDATASTRUCT));
            var result = Marshal.PtrToStringAnsi(cps.lpData);
            return result;
        }
    }
}