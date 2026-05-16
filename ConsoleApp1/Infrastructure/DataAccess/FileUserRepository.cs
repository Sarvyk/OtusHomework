using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Interfaces.DataAccess;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.Json;

namespace ConsoleApp1.Infrastructure.DataAccess
{
    internal class FileUserRepository : IUserRepository
    {
        private readonly string _storagePath;
        public FileUserRepository(string storagePath)
        {
            _storagePath = storagePath;
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
        }
        public async Task AddAsync(ToDoUser user, CancellationToken ct)
        {
            if(!Directory.Exists(Path.Combine(_storagePath, user.UserId.ToString())))
            {
                Directory.CreateDirectory(Path.Combine(_storagePath, user.UserId.ToString()));
            }
            using (FileStream stream = File.Create(Path.Combine(_storagePath, $"{user.UserId}.json")))
            {
                await JsonSerializer.SerializeAsync(stream, user, cancellationToken:ct);
            }
        }

        public Task<IReadOnlyList<ToDoUser>> GetAllUsers(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<ToDoUser?> GetUserAsync(Guid userId, CancellationToken ct)
        {
            string[] users = Directory.GetFiles(_storagePath).Where(name => name == $"{userId.ToString()}.json").ToArray();
            ToDoUser? user = null;
            for (int i = 0; i < users.Length; i++)
            {
                using (FileStream stream = File.OpenRead(users[i]))
                {
                    user = await JsonSerializer.DeserializeAsync<ToDoUser>(stream, cancellationToken: ct);
                }
            }
            return user;
        }

        public async Task<ToDoUser?> GetUserByTelegramUserIdAsync(long telegramUserId, CancellationToken ct)
        {
            string[] users = Directory.GetFiles(_storagePath, "*.json").Where(name => name.LastIndexOf("Indexes.json") == -1).ToArray();
            ToDoUser? user = null;
            for (int i = 0; i<users.Length; i++)
            {
                using (FileStream stream = File.OpenRead(users[i]))
                {
                    user = await JsonSerializer.DeserializeAsync<ToDoUser>(stream,cancellationToken:ct);
                    if (user.TelegramUserId == telegramUserId)
                        return user;
                }
            }
            return user;
        }
    }
}