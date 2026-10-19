using System;

namespace HomeWorkOtusProffesional.Additional
{
    /// <summary>
    /// Дженерик интерфейс для клонирования объектов
    /// </summary>
    /// <typeparam name="T">Тип клонируемого объекта</typeparam>
    public interface IMyCloneable<T>
    {
        /// <summary>
        /// Создает копию текущего объекта
        /// </summary>
        /// <returns>Копия текущего объекта</returns>
        T CloneGeneric();
    }
}