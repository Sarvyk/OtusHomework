using System;

namespace HomeWorkOtusProffesional.Additional
{
    // Пример использования улучшенной архитектуры
    public class EmployeeExample
    {
        public static void RunExample()
        {
            // Создаем человека
            var person = new Person
            {
                Id = 1,
                Name = "Иван Петров",
                Age = 30
            };

            // Создаем компанию
            var company = new Company
            {
                Id = 1,
                Name = "Tech Corp",
                Address = "ул. Примерная, д. 1"
            };

            // Создаем работника как комбинацию человека и компании
            var employee = new Employee
            {
                Person = person,
                Company = company,
                Position = "Программист"
            };

            // Выводим информацию о работнике
            Console.WriteLine("=== Информация о работнике ===");
            employee.DisplayEmployeeInfo();
            
            Console.WriteLine("\n=== Информация о компании ===");
            employee.Company.DisplayCompanyInfo();
            Console.WriteLine("\n=== Тест клонирования ===");

            // Тестируем клонирование через CloneGeneric() (дженерик интерфейс)
            var clonedEmployee1 = employee.CloneGeneric();
            Console.WriteLine("Клон работника (через CloneGeneric()):");
            clonedEmployee1.DisplayEmployeeInfo();

            Console.WriteLine("\nКлон компании (через CloneGeneric()):");
            clonedEmployee1.Company.DisplayCompanyInfo();

            // Тестируем клонирование через Clone() (ICloneable интерфейс)
            var clonedEmployee2 = (Employee)employee.Clone();
            Console.WriteLine("\nКлон работника (через Clone()):");
            clonedEmployee2.DisplayEmployeeInfo();

            Console.WriteLine("\nКлон компании (через Clone()):");
            clonedEmployee2.Company.DisplayCompanyInfo();

            // Проверяем, что это разные объекты
            Console.WriteLine($"\nОригинал и клон (CloneGeneric) - разные объекты: {employee != clonedEmployee1}");
            Console.WriteLine($"Оригинал и клон (CloneGeneric) - разные объекты (Person): {employee.Person != clonedEmployee1.Person}");
            Console.WriteLine($"Оригинал и клон (CloneGeneric) - разные объекты (Company): {employee.Company != clonedEmployee1.Company}");
            
            Console.WriteLine($"\nОригинал и клон (Clone) - разные объекты: {employee != clonedEmployee2}");
            Console.WriteLine($"Оригинал и клон (Clone) - разные объекты (Person): {employee.Person != clonedEmployee2.Person}");
            Console.WriteLine($"Оригинал и клон (Clone) - разные объекты (Company): {employee.Company != clonedEmployee2.Company}");
        }
    }
}