using ConsoleApp1.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Core.Interfaces.DataAccess
{
    internal interface IToDoListService
    {
        Task<ToDoList> Add(ToDoUser user, string name, CancellationToken ct);
        Task<ToDoList?> Get(Guid id, CancellationToken ct);
        Task Delete(Guid id, CancellationToken ct);
        Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken ct);
    }
}
