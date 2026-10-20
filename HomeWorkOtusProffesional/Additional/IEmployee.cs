using System;

namespace HomeWorkOtusProffesional.Additional
{
    // Интерфейс работника, который содержит информацию о человеке и компании
    public interface IEmployee
    {
        Person Person { get; set; }
        Company Company { get; set; }
        string Position { get; set; }

        void DisplayEmployeeInfo();
    }
}