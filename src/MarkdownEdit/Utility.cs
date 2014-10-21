using System;
using System.Collections.Concurrent;
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

        public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 350)
        {
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
    }
}