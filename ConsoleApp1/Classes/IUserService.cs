using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Classes
{
    internal interface IUserService
    {
        ToDoUser RegisterUser(long telegramUserId, string telegramUserName);
        ToDoUser? GetUser(long telegramUserId);
    }
}
