using System.Collections.Generic;

namespace DisposableFixer.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> source, T value)
        {
            foreach (var item in source)
            {
                yield return item;
            }

            yield return value;
        }
    }
}