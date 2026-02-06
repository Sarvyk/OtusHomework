using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.DataAccess
{
    internal interface IToDoReportService
    {
        Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStatsAsync(Guid userId, CancellationToken ct);
    }
}