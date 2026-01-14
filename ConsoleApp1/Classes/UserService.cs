using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Classes
{
    internal class UserService : IUserService
    {
        public ToDoUser? GetUser(long telegramUserId)
        {
            //пока заглушка
            return null;
        }

        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            ToDoUser user = new ToDoUser(telegramUserName, telegramUserId);
            return user;
        }
    }
}