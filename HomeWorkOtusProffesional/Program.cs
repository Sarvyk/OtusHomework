using Classes;
using HomeWorkOtusProffesional.Additional;
using HomeWorkOtusProffesional.Classes;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;

namespace HomeWorkOtusProffesional
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            // Создание трёх файлов со случайным наполнением с пробелами
            Stopwatch stopwatch = Stopwatch.StartNew();
            CreateRandomFiles();
            stopwatch.Stop();
            Console.WriteLine($"Время создания файлов: {stopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine("==============================================================");
            // Чтение файлов параллельно и подсчет пробелов
            TextFileParallelReader reader = new TextFileParallelReader();
            stopwatch.Restart();
            var datas = await reader.ReadFilesParallel("file1.txt", "file2.txt","file3.txt"); // Ждём завершения асинхронного метода
            var result1 = await CountSpacesInFile(datas);
            stopwatch.Stop();
            Console.WriteLine($"Количество пробелов в файлах:");
            WriteAll(result1);
            Console.WriteLine($"Время чтения файлов параллельно: {stopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine("==============================================================");
            // Новая функциональность: чтение всех файлов из папки параллельно
            stopwatch.Restart();
            var datas2 = await reader.ReadAllFilesFromFolder("./"); // Ждём завершения асинхронного метода
            var result2 = await CountSpacesInFile(datas2);
            stopwatch.Stop();
            Console.WriteLine($"Количество пробелов в файлах с аргументом пути к папке:");
            WriteAll(result2);
            Console.WriteLine($"Время чтения всех файлов из папки: {stopwatch.ElapsedMilliseconds} мс");

            // Тестирование параллельного суммирования
            Console.WriteLine("==============================================================");
            Console.WriteLine("Тестирование параллельного суммирования:");
            GetParallelCalc();
            Console.WriteLine("============================Тест классов======================");
            EmployeeExample.RunExample();
            Console.ReadKey();
        }

        static void GetSystemInfo()
        {
            Console.WriteLine("========Характеристики========");
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor"))
            {
                foreach (var obj in searcher.Get())
                {
                    Console.WriteLine($"CPU: {obj["Name"]}");
                    Console.WriteLine($"Ядер: {obj["NumberOfCores"]}, Потоков: {obj["NumberOfLogicalProcessors"]}");
                    Console.WriteLine($"Частота: {obj["MaxClockSpeed"]} MHz");
                }
            }
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_ComputerSystem"))
            {
                foreach (var obj in searcher.Get())
                {
                    ulong totalRam = Convert.ToUInt64(obj["TotalPhysicalMemory"]);
                    Console.WriteLine($"Всего ОЗУ: {totalRam / 1024 / 1024 / 1024} GB");
                }
            }
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
            {
                foreach (var obj in searcher.Get())
                {
                    Console.WriteLine($"ОС: {obj["Caption"]}");
                    Console.WriteLine($"Версия: {obj["Version"]}");
                }
            }
            Console.WriteLine("==============================================================");
        }
        static void GetParallelCalc()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            // Создаем три списка с разными размерами
            List<int> array100k = new List<int>(new int[100000]);
            List<int> array1M = new List<int>(new int[1000000]);
            List<int> array10M = new List<int>(new int[10000000]);

            // Заполняем списки случайными значениями
            Random random = new Random();
            for (int i = 0; i < array100k.Count; i++)
            {
                array100k[i] = random.Next(1, 100);
            }
            for (int i = 0; i < array1M.Count; i++)
            {
                array1M[i] = random.Next(1, 100);
            }
            for (int i = 0; i < array10M.Count; i++)
            {
                array10M[i] = random.Next(1, 100);
            }
            ArraySumCalculator arraySumCalc = new ArraySumCalculator();
            GetSystemInfo();

            // Измеряем время синхронного суммирования
            stopwatch.Restart();
            int sumSync100k = arraySumCalc.CalculateSumSync(array100k);
            stopwatch.Stop();
            Console.WriteLine($"Синхронная сумма (100k элементов): {sumSync100k}, время: {stopwatch.ElapsedMilliseconds} мс");

            stopwatch.Restart();
            int sumSync1M = arraySumCalc.CalculateSumSync(array1M);
            stopwatch.Stop();
            Console.WriteLine($"Синхронная сумма (1M элементов): {sumSync1M}, время: {stopwatch.ElapsedMilliseconds} мс");

            stopwatch.Restart();
            int sumSync10M = arraySumCalc.CalculateSumSync(array10M);
            stopwatch.Stop();
            Console.WriteLine($"Синхронная сумма (10M элементов): {sumSync10M}, время: {stopwatch.ElapsedMilliseconds} мс");

            // Измеряем время параллельного суммирования
            stopwatch.Restart();
            int sumParallel100k = arraySumCalc.CalculateSumParallel(array100k);
            stopwatch.Stop();
            Console.WriteLine($"Параллельная сумма (100k элементов): {sumParallel100k}, время: {stopwatch.ElapsedMilliseconds} мс");

            stopwatch.Restart();
            int sumParallel1M = arraySumCalc.CalculateSumParallel(array1M);
            stopwatch.Stop();
            Console.WriteLine($"Параллельная сумма (1M элементов): {sumParallel1M}, время: {stopwatch.ElapsedMilliseconds} мс");

            stopwatch.Restart();
            int sumParallel10M = arraySumCalc.CalculateSumParallel(array10M);
            stopwatch.Stop();
            Console.WriteLine($"Параллельная сумма (10M элементов): {sumParallel10M}, время: {stopwatch.ElapsedMilliseconds} мс");

            // Измеряем время параллельного суммирования с помощью LINQ
            stopwatch.Restart();
            int sumParallelLinq100k = arraySumCalc.CalculateSumParallelLinq(array100k);
            stopwatch.Stop();
            Console.WriteLine($"Параллельная сумма с LINQ (100k элементов): {sumParallelLinq100k}, время: {stopwatch.ElapsedMilliseconds} мс");

            stopwatch.Restart();
            int sumParallelLinq1M = arraySumCalc.CalculateSumParallelLinq(array1M);
            stopwatch.Stop();
            Console.WriteLine($"Параллельная сумма с LINQ (1M элементов): {sumParallelLinq1M}, время: {stopwatch.ElapsedMilliseconds} мс");

            stopwatch.Restart();
            int sumParallelLinq10M = arraySumCalc.CalculateSumParallelLinq(array10M);
            stopwatch.Stop();
            Console.WriteLine($"Параллельная сумма с LINQ (10M элементов): {sumParallelLinq10M}, время: {stopwatch.ElapsedMilliseconds} мс");
        }
        static void CreateRandomFiles()
        {
            try
            {
                // Создаем три файла с случайным содержимым
                for (int i = 1; i <= 3; i++)
                {
                    string fileName = $"file{i}.txt";
                    string content = GenerateSimpleContent();
                    File.WriteAllText(fileName, content);
                    Console.WriteLine($"Создан файл: {fileName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при создании файлов: {ex.Message}");
            }
        }

        static string GenerateSimpleContent()
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();

            // Генерируем строки
            int linesCount = random.Next(2, 4);

            for (int i = 0; i < linesCount; i++)
            {
                // Каждая строка - случайное количество символов
                int lineLength = random.Next(5, 16);

                for (int j = 0; j < lineLength; j++)
                {
                    // Случайно выбираем, добавлять пробел или символ
                    if (random.Next(0, 3) == 0) // 1 из 3 шансов добавить пробел
                    {
                        sb.Append(' ');
                    }
                    else
                    {
                        // Добавляем случайный символ от 'a' до 'z'
                        char randomChar = (char)random.Next('a', 'z' + 1);
                        sb.Append(randomChar);
                    }
                }

                // Добавляем перевод строки
                sb.AppendLine();
            }

            return sb.ToString();
        }

        static Task<Dictionary<string, int>> CountSpacesInFile(Dictionary<string, string> contents)
        {
            // Подсчитываем количество пробелов
            return Task.Run(() => contents.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Count(c => c == ' ')));
        }
        static void WriteAll(Dictionary<string, int> results)
        {
            foreach(var file in results)
            {
                Console.WriteLine($"Общее количество пробелов в файле {file.Key}: {file.Value}");
            }
        }
    }
}
