using System.Linq;
using System.Collections.Generic;

namespace DisposableFixer.Extensions
{
    internal static class ObjectExtension
    {
        public static IEnumerable<T> Concat<T>(this T item, IEnumerable<T> items)
        {
            return new[] { item }.Concat(items);
        } 
    }
}
