using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartThermo.Core.Extensions
{
    public static class DbLinqExtension
    {
        public static IEnumerable<TSource> TakeLastEx<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector, int N)
        {
            return source.OrderByDescending(selector)
                .Take(N)
                .OrderBy(selector);
        }
    }
}
