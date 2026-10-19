using System;

namespace HomeWorkOtusProffesional.Additional
{
    // Класс работника, который содержит информацию о человеке и компании
    public class Employee : IEmployee, IMyCloneable<Employee>, ICloneable
    {
        public Person Person { get; set; } = null!;
        public Company Company { get; set; } = null!;
        public string Position { get; set; } = null!;

        public void DisplayEmployeeInfo()
        {
            if (Person != null)
            {
                Console.WriteLine($"Employee: {Person.Name} (Id: {Person.Id}), Age: {Person.Age}, Position: {Position}");
            }
            if (Company != null)
            {
                Console.WriteLine($"Company: {Company.Name}, Address: {Company.Address}");
            }
        }

        /// <summary>
        /// Создает копию текущего объекта
        /// </summary>
        /// <returns>Копия текущего объекта</returns>
        public Employee CloneGeneric() => new Employee{ Person = Person.CloneGeneric(),Company = Company.CloneGeneric(),Position = Position };

        /// <summary>
        /// Создает копию текущего объекта (реализация ICloneable)
        /// </summary>
        /// <returns>Копия текущего объекта</returns>
        public object Clone() => this.CloneGeneric();
    }
}