using ConsoleApp1.DataAccess;
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
        public (int total, int completed, int active, DateTime generatedAt) GetUserStats(Guid userId)
        {
            int total = _toDoService.GetAllByUserId(userId).Count;
            int completed = _toDoService.GetAllByUserId(userId).Where(x => x.State == ToDoItemState.Completed).Count();
            int active = _toDoService.GetActiveByUserId(userId).Count;
            DateTime generatedAt = DateTime.Now;
            return (total, completed, active, generatedAt);
        }
    }
}
