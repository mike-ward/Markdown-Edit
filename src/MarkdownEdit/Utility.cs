using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;

namespace MarkdownEdit
{
    internal static class Utility
    {
        public static Func<TKey, TResult> Memoize<TKey, TResult>(this Func<TKey, TResult> func)
        {
            var cache = new ConcurrentDictionary<TKey, TResult>();
            return key => cache.GetOrAdd(key, func);
        }

        public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
        {
            // ReSharper disable once TooWideLocalVariableScope
            T last;
            return arg =>
            {
                last = arg;
                Task.Delay(milliseconds).ContinueWith(t =>
                {
                    if (last.Equals(arg)) func(last);
                    t.Dispose();
                });
            };
        }

        public static void Beep()
        {
            SystemSounds.Beep.Play();
        }

        public static void EditFile(string file)
        {
            Process.Start("Notepad.exe", file);                
        }
    }
}