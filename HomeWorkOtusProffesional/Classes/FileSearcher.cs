using HomeWorkOtusProffesional.Additional;
using System;
using System.Collections.Generic;
using System.Text;

namespace HomeWorkOtusProffesional.Classes
{
    /// <summary>
    /// Класс для поиска файлов в каталоге с возможностью отмены поиска
    /// </summary>
    public class FileSearcher
    {
        /// <summary>
        /// Событие, возникающее при нахождении файла
        /// </summary>
        public event EventHandler<FileArgs> FileFound;

        /// <summary>
        /// Метод для поиска файлов в указанной директории
        /// </summary>
        /// <param name="directoryPath">Путь к директории для поиска</param>
        /// <param name="ct">Токен отмены поиска</param>
        public void SearchFiles(string directoryPath, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(directoryPath))
                throw new ArgumentException("Путь к директории не может быть пустым", nameof(directoryPath));

            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException($"Директория {directoryPath} не найдена");

            SearchDirectory(directoryPath, ct);
        }

        /// <summary>
        /// Рекурсивный метод поиска файлов в директории
        /// </summary>
        /// <param name="directoryPath">Путь к директории для поиска</param>
        private void SearchDirectory(string directoryPath, CancellationToken ct)
        {
            try
            {
                // Получаем все файлы в текущей директории
                string[] files = Directory.GetFiles(directoryPath);

                foreach (string file in files)
                {
                    ct.ThrowIfCancellationRequested();
                    // Генерируем событие при нахождении файла
                    OnFileFound(file);
                }

                // Получаем все поддиректории
                string[] subDirectories = Directory.GetDirectories(directoryPath);

                foreach (string subDirectory in subDirectories)
                {
                    ct.ThrowIfCancellationRequested();
                    SearchDirectory(subDirectory, ct);
                }
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (UnauthorizedAccessException)
            {
                // Пропускаем директории, к которым нет доступа
                Console.WriteLine($"Нет доступа к директории: {directoryPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при поиске в директории {directoryPath}: {ex.Message}");
            }
        }

        /// <summary>
        /// Метод для генерации события при нахождении файла
        /// </summary>
        /// <param name="fileName">Имя найденного файла</param>
        protected virtual void OnFileFound(string fileName)
        {
            // Создаем аргументы события
            var args = new FileArgs(fileName);

            // Вызываем событие
            FileFound?.Invoke(this, args);
        }
    }
}
