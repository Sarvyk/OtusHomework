using System;
using System.Collections.Generic;
using System.Text;

namespace HomeWorkOtusProffesional.Additional
{
    /// <summary>
    /// Аргументы события для найденного файла
    /// </summary>
    public class FileArgs : EventArgs
    {
        /// <summary>
        /// Имя найденного файла
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Конструктор аргументов события
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        public FileArgs(string fileName)
        {
            FileName = fileName;
        }
    }
}
