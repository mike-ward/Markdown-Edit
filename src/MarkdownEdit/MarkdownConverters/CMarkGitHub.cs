using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MarkdownEdit.MarkdownConverters
{
    public class CMarkGitHub : IMarkdownConverter
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int CoreExtensionsRegistrationDelegate([MarshalAs(UnmanagedType.LPStruct)] IntPtr plugIntPtr);

        [DllImport("cmark.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void cmark_register_plugin(CoreExtensionsRegistrationDelegate cerd);

        [DllImport("cmark.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int core_extensions_registration([MarshalAs(UnmanagedType.LPStruct)] IntPtr plugIntPtr);

        [DllImport("cmark.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cmark_markdown_to_html(
            [In, MarshalAs(UnmanagedType.LPStr)] string text,
            [MarshalAs(UnmanagedType.U4)] uint len,
            [MarshalAs(UnmanagedType.I4)] int options);

        [DllImport("cmark.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cmark_parser_new([MarshalAs(UnmanagedType.I4)] int options);

        [DllImport("cmark.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr cmark_find_syntax_extension([In, MarshalAs(UnmanagedType.LPStr)] string extension);

        public string ConvertToHtml(string markdown)
        {
            var del = new CoreExtensionsRegistrationDelegate(core_extensions_registration); 
            cmark_register_plugin(del);

            var CMARK_OPT_HARDBREAKS = 1 << 2;
            var CMARK_OPT_NORMALIZE =  1 << 8;

            var parser = cmark_parser_new(CMARK_OPT_NORMALIZE | CMARK_OPT_HARDBREAKS);
            if (parser == IntPtr.Zero) throw new ApplicationException("cmark_parser_new error");

            var tableExtension = cmark_find_syntax_extension("table");
            if (tableExtension == IntPtr.Zero) throw new ApplicationException("can't find table extension");

            var length = (uint) Encoding.UTF8.GetByteCount(markdown);
            var retValue = cmark_markdown_to_html(markdown, length, 0);
            return Marshal.PtrToStringAnsi(retValue);
        }
    }
}
