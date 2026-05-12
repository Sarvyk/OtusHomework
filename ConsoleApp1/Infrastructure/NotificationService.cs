using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Interfaces.DataAccess;
using ConsoleApp1.Infrastructure.DataAccess;
using ConsoleApp1.Infrastructure.DataAccess.Models;
using ConsoleApp1.Infrastructure.Interfaces;
using LinqToDB;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ConsoleApp1.Infrastructure
{
    internal class NotificationService : INotificationService
    {
        private readonly IDataContextFactory<ToDoDataContext> _factory;
        public NotificationService(IDataContextFactory<ToDoDataContext> dataContextFactory)
        {
            _factory = dataContextFactory;
        }
        public async Task<IReadOnlyList<Notification>> GetScheduledNotification(DateTime scheduledBefore, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var models = await AsyncExtensions.ToListAsync(dbContext.Notifications
                .Where(n => !n.IsNotified && n.ScheduledAt <= scheduledBefore)
                .LoadWith(n => n.User));
            return models.Select(ModelMapper.MapFromModel).ToList();
        }

        public async Task MarkNotified(Guid notificationId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var model = await dbContext.Notifications.FirstOrDefaultAsync(n => n.ExternalId == notificationId);
            if (model == null)
                throw new Exception("Такого уведомления не существует");
            model.IsNotified = true;
            model.NotifiedAt = DateTime.UtcNow;
            await dbContext.UpdateAsync(model, token: ct);
        }

        public async Task<bool> ScheduleNotification(Guid userId, string type, string text, DateTime scheduledAt, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var user = await dbContext.ToDoUsers.FirstOrDefaultAsync(u => u.ExternalId == userId);
            if (await dbContext.Notifications.AnyAsync(n => n.UserId == user.Id && n.Type == text))
                return false;
            Notification notification = new Notification()
            {
                Id = Guid.NewGuid(),
                User = ModelMapper.MapFromModel(user),
                Type = type,
                Text = text,
                ScheduledAt = scheduledAt
            };
            NotificationModel model = ModelMapper.MapToModel(notification);
            await dbContext.InsertAsync(model);
            return true;
        }
    }
}