using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class Utility
    {
        public static Func<TKey, TResult> Memoize<TKey, TResult>(this Func<TKey, TResult> func)
        {
            var cache = new ConcurrentDictionary<TKey, TResult>();
            return key => cache.GetOrAdd(key, func);
        }

        public static Action Debounce(this Action func, int milliseconds = 300)
        {
            var action = Debounce<int>(_ => func(), milliseconds);
            return () => action(0);
        }

        public static Action<T> Debounce<T>(this Action<T> func, int milliseconds = 300)
        {
            var last = long.MinValue;
            return arg =>
            {
                try
                {
                    var current = Interlocked.Increment(ref last);
                    Task.Delay(milliseconds).ContinueWith(task =>
                    {
                        // ReSharper disable once AccessToModifiedClosure
                        if (current == last) func(arg);
                        task.Dispose();
                    });
                }
                catch (OverflowException)
                {
                    Interlocked.Exchange(ref last, long.MinValue);
                }
            };
        }
    }
}