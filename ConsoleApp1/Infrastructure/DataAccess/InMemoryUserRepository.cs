using ConsoleApp1.DataAccess;
using ConsoleApp1.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Infrastructure.DataAccess
{
    internal class InMemoryUserRepository : IUserRepository
    {
        private readonly List<ToDoUser> _users = new List<ToDoUser>();
        public async Task AddAsync(ToDoUser user, CancellationToken ct)
        {
            _users.Add(user);
        }

        public async Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken ct)
        {
            return _users.FirstOrDefault(x => x.UserId == userId);
        }

        public async Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId,CancellationToken ct)
        {
            return _users.FirstOrDefault(x => x.TelegramUserId == telegramUserId);
        }
    }
}
