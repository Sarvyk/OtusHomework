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
        public void Add(ToDoUser user)
        {
            _users.Add(user);
        }

        public ToDoUser? GetUser(Guid userId)
        {
            return _users.FirstOrDefault(x => x.UserId == userId);
        }

        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            return _users.FirstOrDefault(x => x.TelegramUserId == telegramUserId);
        }
    }
}
