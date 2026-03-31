using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ConsoleApp1.Helpers
{
    internal static class EnumerableExtension
    {
        public static IEnumerable<T> GetBatchByNumber<T>(this IEnumerable<T> source, int batchSize, int batchNumber)
        {
            if(source == null)
                throw new ArgumentNullException(nameof(source));
            if (batchSize < 0 || batchNumber < 0)
                throw new ArgumentOutOfRangeException("Размер последовательности или позиция не может быть меньше нуля");
            return source.Skip(batchSize*batchNumber).Take(batchSize);
        }
    }
}