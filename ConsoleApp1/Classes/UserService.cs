using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Classes
{
    internal class UserService : IUserService
    {
        private readonly List<ToDoUser> _users = new List<ToDoUser>();
        public ToDoUser? GetUser(long telegramUserId)
        {
            ToDoUser? User = _users.FirstOrDefault(user => user.TelegramUserId == telegramUserId);
            if (User != null)
                return User;
            else
                return null;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            ToDoUser user = new ToDoUser(telegramUserName, telegramUserId);
            _users.Add(user);
            return user;
        }
    }
}