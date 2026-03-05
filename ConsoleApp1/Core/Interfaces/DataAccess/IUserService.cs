using ConsoleApp1.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Core.Interfaces.DataAccess
{
    internal interface IUserService
    {
        Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName, CancellationToken ct);
        Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken ct);
        Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken ct);
    }
}