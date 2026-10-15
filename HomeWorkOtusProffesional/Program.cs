using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;

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
            stopwatch.Restart();
            await ReadFilesParallel(); // Ждём завершения асинхронного метода
            stopwatch.Stop();
            Console.WriteLine($"Время чтения файлов параллельно: {stopwatch.ElapsedMilliseconds} мс");
            Console.WriteLine("==============================================================");
            // Новая функциональность: чтение всех файлов из папки параллельно
            stopwatch.Restart();
            await ReadAllFilesFromFolder("./"); // Ждём завершения асинхронного метода
            stopwatch.Stop();
            Console.WriteLine($"Время чтения всех файлов из папки: {stopwatch.ElapsedMilliseconds} мс");
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
        
        static async Task ReadFilesParallel()
        {
            try
            {
                // Оптимизированный вариант: используем LINQ для создания задач, как во втором методе
                string[] files = {"file1.txt", "file2.txt", "file3.txt"};
                Task<int>[] tasks = files.Select(file => CountSpacesInFile(file)).ToArray();
                
                // Ожидаем завершения всех задач
                int[] results = await Task.WhenAll(tasks);
                
                // Выводим результаты
                Console.WriteLine($"\nКоличество пробелов в файлах:");
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{Path.GetFileName(files[i])}: {results[i]}");
                }
                
                int totalSpaces = results.Sum();
                Console.WriteLine($"\nОбщее количество пробелов: {totalSpaces}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файлов: {ex.Message}");
            }
        }
        
        static async Task<int> CountSpacesInFile(string fileName)
        {
            try
            {
                // Читаем содержимое файла асинхронно
                string content = await File.ReadAllTextAsync(fileName);
                
                // Подсчитываем количество пробелов
                int spaceCount = content.Count(c => c == ' ');
                
                return spaceCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла {fileName}: {ex.Message}");
                return 0;
            }
        }
        
        // Новая функция для чтения всех файлов из папки параллельно
        static async Task ReadAllFilesFromFolder(string folderPath)
        {
            try
            {
                // Получаем список файлов в папке
                string[] files = Directory.GetFiles(folderPath, "*.txt");
                
                if (files.Length == 0)
                {
                    Console.WriteLine($"\nВ папке {folderPath} нет текстовых файлов.");
                    return;
                }
                
                Console.WriteLine($"\nНайдено {files.Length} текстовых файлов в папке {folderPath}:");
                
                // Создаем задачи для чтения всех файлов параллельно
                Task<int>[] tasks = files.Select(file => CountSpacesInFile(file)).ToArray();
                
                // Ожидаем завершения всех задач
                int[] results = await Task.WhenAll(tasks);
                
                // Выводим результаты
                Console.WriteLine($"\nКоличество пробелов в файлах:");
                for (int i = 0; i < files.Length; i++)
                {
                    Console.WriteLine($"{Path.GetFileName(files[i])}: {results[i]}");
                }
                
                int totalSpaces = results.Sum();
                Console.WriteLine($"\nОбщее количество пробелов: {totalSpaces}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файлов из папки {folderPath}: {ex.Message}");
            }
        }
    }
}
