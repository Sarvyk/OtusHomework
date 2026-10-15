using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Classes
{
    public class ArraySumCalculator
    {
        private int sleep = 0;
        /// <summary>
        /// Синхронный метод для подсчёта суммы элементов списка целых чисел
        /// </summary>
        /// <param name="list">Список целых чисел</param>
        /// <returns>Сумма элементов списка</returns>
        public int CalculateSumSync(List<int> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            int sum = 0;
            foreach (int number in list)
            {
                sum += number;
                //Thread.Sleep(sleep);
            }
            
            return sum;
        }

        /// <summary>
        /// Параллельный метод для подсчёта суммы элементов списка целых чисел
        /// </summary>
        /// <param name="list">Список целых чисел</param>
        /// <returns>Сумма элементов списка</returns>
        public int CalculateSumParallel(List<int> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count == 0)
                return 0;

            // Определяем количество потоков для параллелизма
            int threadCount = Environment.ProcessorCount;
            
            // Если список маленький, используем один поток
            if (list.Count < threadCount)
                threadCount = 1;

            int chunkSize = list.Count / threadCount;
            int remainder = list.Count % threadCount;

            // Создаем массив для хранения промежуточных результатов
            int[] results = new int[threadCount];
            Thread[] threads = new Thread[threadCount];

            // Создаем и запускаем потоки
            for (int i = 0; i < threadCount; i++)
            {
                int startIndex = i * chunkSize + (i < remainder ? i : remainder);
                int currentChunkSize = chunkSize + (i < remainder ? 1 : 0);

                // Используем замыкание для передачи параметров в поток
                int index = i;
                int start = startIndex;
                int size = currentChunkSize;

                threads[i] = new Thread(() =>
                {
                    int sum = 0;
                    for (int j = 0; j < size; j++)
                    {
                        sum += list[start + j];
                        //Thread.Sleep(sleep);
                    }
                    results[index] = sum;
                });
                
                threads[i].Start();
            }

            // Ожидаем завершения всех потоков
            foreach (Thread thread in threads)
            {
                thread.Join();
            }

            // Суммируем результаты из всех потоков
            int totalSum = 0;
            foreach (int result in results)
            {
                totalSum += result;
            }

            return totalSum;
        }

        /// <summary>
        /// Параллельный метод для подсчёта суммы элементов списка целых чисел с помощью LINQ
        /// </summary>
        /// <param name="list">Список целых чисел</param>
        /// <returns>Сумма элементов списка</returns>
        public int CalculateSumParallelLinq(List<int> list)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Count == 0)
                return 0;

            // Используем ParallelEnumerable для параллельного вычисления суммы
            return list.AsParallel().Sum(x => x);
        }
    }
}