using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Exceptions;
using ConsoleApp1.Core.Interfaces.DataAccess;

namespace ConsoleApp1.Core.Services
{
    internal class ToDoListService : IToDoListService
    {
        private readonly IToDoListRepository _repository;
        public ToDoListService(IToDoListRepository repository)
        {
            _repository = repository;
        }
        public async Task<ToDoList> Add(ToDoUser user, string name, CancellationToken ct)
        {
            if (name.Length > 10)
                throw new ArgumentException("Слишком длинное название. Допускается название максимум из 10 символов!");
            if (await _repository.ExistsByName(user.UserId, name,ct))
                throw new DublicateListException(name);
            ToDoList toDoList = new ToDoList(user, name);
            await _repository.Add(toDoList, ct);
            return toDoList;
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            await _repository.Delete(id, ct);
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            return await _repository.Get(id, ct);
        }

        public async Task<IReadOnlyList<ToDoList>> GetUserLists(Guid userId, CancellationToken ct)
        {
            return await _repository.GetByUserId(userId, ct);
        }
    }
}
