using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Classes
{
    internal class ToDoUser
    {
        Guid UserId;
        public string TelegramUserName;
        DateTime RegisteredAt;
        public ToDoUser(string telegramUserName)
        {
            UserId = Guid.NewGuid();
            TelegramUserName = telegramUserName;
            RegisteredAt = DateTime.UtcNow;
        }
    }
}
