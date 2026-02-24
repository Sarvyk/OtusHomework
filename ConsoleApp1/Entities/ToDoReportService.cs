using ConsoleApp1.DataAccess;
using ConsoleApp1.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Entities
{
    internal class ToDoReportService : IToDoReportService
    {
        private readonly IToDoService _toDoService;
        public ToDoReportService(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }
        public async Task<(int total, int completed, int active, DateTime generatedAt)> GetUserStatsAsync(Guid userId, CancellationToken ct)
        {
            var allItemsTask = _toDoService.GetAllByUserIdAsync(userId, ct);
            var activeItemsTask = _toDoService.GetActiveByUserIdAsync(userId, ct);
            await Task.WhenAll(allItemsTask, activeItemsTask);
            int total = allItemsTask.Result.Count;
            int completed = allItemsTask.Result.Where(x => x.State == ToDoItemState.Completed).Count();
            int active = activeItemsTask.Result.Count;
            DateTime generatedAt = DateTime.Now;
            return (total, completed, active, generatedAt);
        }
    }
}