using ConsoleApp1.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Core.Interfaces.DataAccess
{
    internal interface IToDoRepository
    {
        Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct);
        //Возвращает ToDoItem для UserId со статусом Active
        Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct);
        Task<IReadOnlyList<ToDoItem>> GetActiveWithDeadline(Guid userId, DateTime from, DateTime to, CancellationToken ct);
        Task<IReadOnlyList<ToDoItem>> GetCompletedByUserIdAsync(Guid userId, CancellationToken ct);
        Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct);
        Task<ToDoItem?> GetAsync(Guid id, CancellationToken ct);
        Task AddAsync(ToDoItem item, CancellationToken ct);
        Task UpdateAsync(ToDoItem item, CancellationToken ct);
        Task DeleteAsync(Guid id, CancellationToken ct);
        //Проверяет есть ли задача с таким именем у пользователя
        Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct);
        //Возвращает количество активных задач у пользователя
        Task<int> CountActiveAsync(Guid userId, CancellationToken ct);
    }
}