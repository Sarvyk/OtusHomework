using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Interfaces.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;

namespace ConsoleApp1.BackgroundTasks
{
    internal class NotificationBackgroundTask : BackgroundTask
    {
        //private readonly TimeSpan _resetScenarioTimeout;
        private readonly INotificationService _notificationService;
        private readonly ITelegramBotClient _botClient;
        public NotificationBackgroundTask(TimeSpan delay, INotificationService notificationService, ITelegramBotClient botClient) : base(delay, nameof(NotificationBackgroundTask))
        {
            _notificationService = notificationService;
            _botClient = botClient;
        }
        protected override async Task Execute(CancellationToken ct)
        {
            var notifications = await _notificationService.GetScheduledNotification(DateTime.UtcNow, ct);
            foreach(Notification notif in notifications)
            {
                await _botClient.SendMessage(notif.User.TelegramUserId, notif.Text, cancellationToken:ct);
                await _notificationService.MarkNotified(notif.Id, ct);
            }
        }
    }
}