using ConsoleApp1.Entities;

namespace ConsoleApp1.DataAccess
{
    internal interface IUserRepository
    {
        Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken ct);
        Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken ct);
        Task AddAsync(ToDoUser user, CancellationToken ct);
    }
}