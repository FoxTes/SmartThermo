using System.Collections.Generic;

namespace SmartThermo.Core.Extensions
{
    /// <summary>
    /// Возвращает строковый массив, содержащий подстроки данного экземпляра,
    /// разделенные элементами заданной строки или массива знаков Юникода.
    /// </summary>
    public static class SplitByExtension
    {
        /// <summary>
        /// Разбивает входную коллекцию на максимальное число подколлекций
        /// на основе указанных символов-разделителей.
        /// </summary>
        /// <param name="source">Входная коллекция.</param>
        /// <param name="delimiter">Разделитель.</param>
        /// <typeparam name="T">Тип коллекции.</typeparam>
        /// <returns>Выходные коллекции.</returns>
        public static IEnumerable<List<T>> SplitBy<T>(this IEnumerable<T> source, T delimiter)
        {
            var buffer = new List<T>();
            var comparer = EqualityComparer<T>.Default;

            foreach (var item in source)
            {
                if (comparer.Equals(item, delimiter))
                {
                    if (buffer.Count > 0)
                        yield return buffer;
                    buffer = new List<T>();
                }
                else
                {
                    buffer.Add(item);
                }
            }

            yield return buffer;
        }
    }
}