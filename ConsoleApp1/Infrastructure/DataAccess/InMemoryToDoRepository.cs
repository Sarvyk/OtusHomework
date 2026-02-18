using ConsoleApp1.DataAccess;
using ConsoleApp1.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Infrastructure.DataAccess
{
    internal class InMemoryToDoRepository : IToDoRepository
    {
        private readonly List<ToDoItem> _items = new List<ToDoItem>();
        public async Task AddAsync(ToDoItem item, CancellationToken ct)
        {
            _items.Add(item);
        }

        public async Task<int> CountActiveAsync(Guid userId, CancellationToken ct)
        {
            return _items.Count(x => x.State == ToDoItemState.Active);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            ToDoItem? item = _items.FirstOrDefault(x => x.id == id);
            if(item != null)
                _items.Remove(item);
            else
                throw new ArgumentException("Такой задачи нет, либо список пуст");
        }

        public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct)
        {
            return _items.Any(x => x.User.UserId == userId && x.Name == name);
        }

        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var items = _items.Where(x => x.User.UserId == userId).ToList();
            return items.Where(predicate).ToList();
        }

        public async Task<ToDoItem?> GetAsync(Guid userId, Guid id, CancellationToken ct)
        {
            return _items.FirstOrDefault(x => x.id == id);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return _items.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return _items.Where(x => x.User.UserId == userId).ToList();
        }

        public async Task UpdateAsync(ToDoItem item, CancellationToken ct)
        {
            ToDoItem? task = _items.Find(x => x == item);
            if (task != null)
                task.State = ToDoItemState.Completed;
            else 
                throw new ArgumentException("Такой задачи нет, либо список пуст");
        }
    }
}