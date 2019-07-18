using System;
using System.Collections.Generic;
using System.Linq;

namespace ESFA.DC.ILR.ReportService.Reports.Extensions
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        public static int DistinctByCount<TSource>(this IEnumerable<TSource> source, Func<TSource, string> keySelector)
        {
            HashSet<string> seenKeys = new HashSet<string>();
            foreach (TSource element in source)
            {
                string val = keySelector(element);
                if (!string.IsNullOrEmpty(val))
                {
                    seenKeys.Add(val);
                }
            }

            return seenKeys.Count;
        }

        public static T MaxOrDefault<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.DefaultIfEmpty().Max();
        }

        public static TResult MaxOrDefault<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> func)
        {
            return enumerable.Select(func).DefaultIfEmpty().Max();
        }

        public static TResult MinOrDefault<T, TResult>(this IEnumerable<T> enumerable, Func<T, TResult> func)
        {
            return enumerable.Select(func).DefaultIfEmpty().Min();
        }

        public static T[] ToFixedLengthArray<T>(this IEnumerable<T> enumerable, int size)
        {
            var array = new T[size];

            var collection = enumerable?.ToArray() ?? Array.Empty<T>();

            var iterations = Math.Min(size, collection.Length);

            for (var i = 0; i < iterations; i++)
            {
                array[i] = collection[i];
            }

            return array;
        }
    }
}
