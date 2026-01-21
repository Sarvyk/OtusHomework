using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Classes
{
    internal interface IToDoService
    {
        IReadOnlyList<ToDoItem> GetAllByUserId(Guid userid);
        IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId);
        ToDoItem Add(ToDoUser user, string name);
        void MarkCompleted(Guid id);
        void Delete(Guid id);
    }
}
