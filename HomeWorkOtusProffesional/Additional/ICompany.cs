using System;

namespace HomeWorkOtusProffesional.Additional
{
    // Интерфейс компании
    public interface ICompany
    {
        string Address { get; set; }
        void DisplayCompanyInfo();
    }
}