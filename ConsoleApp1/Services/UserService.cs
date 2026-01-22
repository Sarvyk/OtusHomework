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
        public ToDoUser? GetUser(Guid userId)
        {
            return _repository.GetUser(userId);
        }
        public ToDoUser? GetUserByTelegramUserId(long telegramUserId)
        {
            return _repository.GetUserByTelegramUserId(telegramUserId);
        }
        public ToDoUser RegisterUser(long telegramUserId, string telegramUserName)
        {
            ToDoUser user = new ToDoUser(telegramUserName, telegramUserId);
            _repository.Add(user);
            return user;
        }
    }
}