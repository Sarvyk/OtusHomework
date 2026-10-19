using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeWorkOtusProffesional.Classes
{
    public class TextFileParallelReader
    {
        /// <summary>
        /// Параллельное чтение файлов через LINQ
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> ReadFilesParallel(params string[] files)
        {
            Dictionary<string,string> results = new Dictionary<string, string>();
            try
            {
                // Оптимизированный вариант: используем LINQ для создания задач, как во втором методе

                var tasks = files.Select(async file =>
                {
                    return new KeyValuePair<string, string>(file, await ReadFiles(file));
                });

                // Ожидаем завершения всех задач
                var taskResults = await Task.WhenAll(tasks);
                results = taskResults.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файлов: {ex.Message}");
            }
            return results;
        }
        /// <summary>
        /// Параллельное чтение файлов через Tasks
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> ReadAllFilesFromFolder(string folderPath)
        {
            Dictionary<string, string> results = new Dictionary<string, string>();
            try
            {
                // Получаем список файлов в папке
                string[] files = Directory.GetFiles(folderPath, "*.txt");

                if (files.Length == 0)
                {
                    return results;
                }
                // Создаем задачи для чтения всех файлов параллельно
                var tasks = files.Select(async file =>
                {
                    var data = await ReadFiles(file);
                    return new KeyValuePair<string, string>(file, data);
                });

                // Ожидаем завершения всех задач
                var taskResults = await Task.WhenAll(tasks);
                results = taskResults.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файлов из папки {folderPath}: {ex.Message}");
            }
            return results;
        }
        private async Task<string> ReadFiles(string file)
        {
            return await File.ReadAllTextAsync(file);
        }
    }
}