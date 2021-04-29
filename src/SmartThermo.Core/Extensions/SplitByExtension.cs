using System.Collections.Generic;

namespace SmartThermo.Core.Extensions
{
    public static class SplitByExtension
    {
        public static IEnumerable<List<T>> SplitBy<T>(this IEnumerable<T> source, T delimiter)
        {
            var buffer = new List<T>();
            var comparer = EqualityComparer<T>.Default;
            
            foreach (var item in source)
                if (comparer.Equals(item, delimiter))
                {
                    if (buffer.Count > 0)
                        yield return buffer;
                    buffer = new List<T>();
                }
                else
                    buffer.Add(item);
            
            yield return buffer;
        }
    }
}