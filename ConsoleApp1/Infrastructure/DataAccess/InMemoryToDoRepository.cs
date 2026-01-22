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
        public void Add(ToDoItem item)
        {
            _items.Add(item);
        }

        public int CountActive(Guid userId)
        {
            return _items.Count(x => x.State == ToDoItemState.Active);
        }

        public void Delete(Guid id)
        {
            ToDoItem? item = _items.FirstOrDefault(x => x.id == id);
            if(item != null)
                _items.Remove(item);
            else
                throw new ArgumentException("Такой задачи нет, либо список пуст");
        }

        public bool ExistsByName(Guid userId, string name)
        {
            return _items.Any(x => x.User.UserId == userId && x.Name == name);
        }

        public IReadOnlyList<ToDoItem> Find(Guid userId, Func<ToDoItem, bool> predicate)
        {
            var items = _items.Where(x => x.User.UserId == userId).ToList();
            return items.Where(predicate).ToList();
        }

        public ToDoItem? Get(Guid id)
        {
            return _items.FirstOrDefault(x => x.id == id);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            return _items.Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList();
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userId)
        {
            return _items.Where(x => x.User.UserId == userId).ToList();
        }

        public void Update(ToDoItem item)
        {
            ToDoItem? task = _items.Find(x => x == item);
            if (task != null)
                task.State = ToDoItemState.Completed;
            else 
                throw new ArgumentException("Такой задачи нет, либо список пуст");
        }
    }
}