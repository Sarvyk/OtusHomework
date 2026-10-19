using System;

namespace HomeWorkOtusProffesional.Additional
{
    // Интерфейс человека
    public interface IPerson
    {
        int Age { get; set; }
        void DisplayPersonInfo();
    }
}