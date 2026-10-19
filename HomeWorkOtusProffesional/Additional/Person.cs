using System;

namespace HomeWorkOtusProffesional.Additional
{
    // Класс человека
    public class Person : Entity, IPerson, ICloneable
    {
        public int Age { get; set; }

        public virtual void DisplayPersonInfo() => Console.WriteLine($"Person: {Name}, Age: {Age}");

        /// <summary>
        /// Создает копию текущего объекта
        /// </summary>
        /// <returns>Копия текущего объекта</returns>
        public override Person CloneGeneric() => new Person { Id = this.Id,Name = this.Name,Age = this.Age };
    }
}