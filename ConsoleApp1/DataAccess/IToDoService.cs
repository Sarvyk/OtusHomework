using ConsoleApp1.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.DataAccess
{
    internal interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userid);
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        IReadOnlyList<ToDoItem> Find(Guid userId, string namePrefix);
        ToDoItem Add(ToDoUser user, string name);
        void MarkCompleted(Guid id);
        void Delete(Guid id);
    }
}
