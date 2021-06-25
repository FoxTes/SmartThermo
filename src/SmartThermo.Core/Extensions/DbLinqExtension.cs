using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartThermo.Core.Extensions
{
    /// <summary>
    /// Расширение для работы с БД.
    /// </summary>
    public static class DbLinqExtension
    {
        /// <summary>
        /// Возвращает указанное количество смежных элементов с конца наблюдаемой последовательности.
        /// </summary>
        /// <param name="source">Исходная последовательность.</param>
        /// <param name="selector">Элемент по которому производится сортировка.</param>
        /// <param name="n">Число элементов, которые нужно взять из конца исходной последовательности.</param>
        /// <typeparam name="TSource">Тип источника.</typeparam>
        /// <typeparam name="TResult">Тип результата.</typeparam>
        /// <returns>Наблюдаемая последовательность,
        /// содержащая указанное количество элементов из исходной последовательности.</returns>
        public static IEnumerable<TSource> TakeLastEx<TSource, TResult>(
            this IEnumerable<TSource> source,
            Func<TSource, TResult> selector,
            int n)
        {
            return source.OrderByDescending(selector)
                .Take(n)
                .OrderBy(selector);
        }
    }
}
