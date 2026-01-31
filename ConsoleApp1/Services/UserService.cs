using ConsoleApp1.DataAccess;
using ConsoleApp1.Entities;
using ConsoleApp1.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository userRepository)
        {
            _repository = userRepository;
        }
        public async Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken ct)
        {
            return await _repository.GetUserAsync(userId, ct);
        }
        public async Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken ct)
        {
            return await _repository.GetUserByTelegramUserIdAsync(telegramUserId, ct);
        }
        public async Task<ToDoUser> RegisterUserAsync(long telegramUserId, string telegramUserName, CancellationToken ct)
        {
            ToDoUser user = new ToDoUser(telegramUserName, telegramUserId);
            await _repository.AddAsync(user, ct);
            return user;
        }
    }
}