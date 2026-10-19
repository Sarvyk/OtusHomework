using System;

namespace HomeWorkOtusProffesional.Additional
{
    // Класс компании
    public class Company : Entity, ICompany
    {
        public string Address { get; set; } = null!;

        public virtual void DisplayCompanyInfo() => Console.WriteLine($"Company: {Name}, Address: {Address}");

        /// <summary>
        /// Создает копию текущего объекта
        /// </summary>
        /// <returns>Копия текущего объекта</returns>
        public override Company CloneGeneric() => new Company { Id = this.Id,Name = this.Name,Address = this.Address };
    }
}