using System;

namespace HomeWorkOtusProffesional.Additional
{
    // Базовый класс для сущностей
    public class Entity : IMyCloneable<Entity>, ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public virtual void DisplayInfo() => Console.WriteLine($"Entity: {Name} (Id: {Id})");

        /// <summary>
        /// Создает копию текущего объекта
        /// </summary>
        /// <returns>Копия текущего объекта</returns>
        public virtual Entity CloneGeneric() => new Entity { Id = this.Id, Name = this.Name };

        /// <summary>
        /// Создает копию текущего объекта (реализация ICloneable)
        /// </summary>
        /// <returns>Копия текущего объекта</returns>
        public object Clone() => this.CloneGeneric();
    }
}