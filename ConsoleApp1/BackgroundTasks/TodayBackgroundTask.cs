using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Interfaces.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.BackgroundTasks
{
    internal class TodayBackgroundTask : BackgroundTask
    {
        private readonly INotificationService _notificationService;
        private readonly IUserRepository _userRepository;
        private readonly IToDoRepository _toDoRepository;
        public TodayBackgroundTask(TimeSpan delay, INotificationService notificationService, IUserRepository userRepository, IToDoRepository toDoRepository) : base(delay, nameof(TodayBackgroundTask))
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
            _toDoRepository = toDoRepository;
        }

        protected override async Task Execute(CancellationToken ct)
        {
            var users = await _userRepository.GetAllUsers(ct);
            foreach (var user in users)
            {
                var tasks = await _toDoRepository.GetActiveWithDeadline(user.UserId, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, ct);
                string taskList = string.Empty;
                foreach(var task in tasks)
                {
                    taskList += $"{task.Id}){task.Name}\r\n";
                }
                if(tasks.Count > 0)
                    await _notificationService.ScheduleNotification(user.UserId, $"Today_{DateOnly.FromDateTime(DateTime.UtcNow)}",taskList, DateTime.UtcNow, ct);
            }
        }
    }
}