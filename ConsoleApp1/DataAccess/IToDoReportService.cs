using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.DataAccess
{
    internal interface IToDoReportService
    {
        (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId);
    }
}
