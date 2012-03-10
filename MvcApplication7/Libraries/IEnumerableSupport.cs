using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Daruyanagi.Libraries
{
    public static class IEnumerableSupport
    {
        public static IEnumerable<T> Hook<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
                yield return item;
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }
    }
}