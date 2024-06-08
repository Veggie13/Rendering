using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParallelExtensions
{
    public static class ParallelExtensions
    {
        public static IEnumerable<T> LazyAwait<T>(this IEnumerable<Task<T>> tasks) => tasks.Select(t => t.Result);

        public static Task ForAllAsync<T>(this ParallelQuery<T> items, Action<T> action)
        {
            return Task.Run(() => items.ForAll(action));
        }

        public static void AwaitAll(this IEnumerable<Task> tasks)
        {
            tasks.AsParallel().ForAll(t => t.Wait());
        }

        public static IEnumerable<T> AwaitAll<T>(this IEnumerable<Task<T>> tasks)
        {
            return tasks.AsParallel().Select(t =>
                {
                    t.Wait();
                    return t.Result;
                });
        }

        public static IEnumerable<V> AwaitAll<U, T, V>(this IEnumerable<U> items, Func<U, Task<T>> taskSelector, Func<U, T, V> outputSelector)
        {
            return items.AsParallel()
                .Select(i => new { Item = i, Task = taskSelector(i) })
                .Select(a =>
                {
                    a.Task.Wait();
                    return outputSelector(a.Item, a.Task.Result);
                });
        }
    }
}
