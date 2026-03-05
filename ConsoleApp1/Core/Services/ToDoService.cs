using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Exceptions;
using ConsoleApp1.Core.Interfaces.DataAccess;

namespace ConsoleApp1.Core.Services
{
    internal class ToDoService : IToDoService
    {
        private readonly IToDoRepository _repository;
        private int? _maxTasks;
        private int? _maxTaskLength;
        public ToDoService(IToDoRepository repository)
        {
            _repository = repository;
            SetMaxTask();
            SetMaxTaskLength();
        }
        public async Task<ToDoItem> AddAsync(ToDoUser user, string name, DateTime deadLine, CancellationToken ct)
        {
            ValidateString(name);
            ToDoItem task = new ToDoItem(user, name, deadLine);
            if(task.Name.Length > _maxTaskLength)
            {
                throw new TaskLenghtLimitException(task.Name.Length, (int)_maxTaskLength);
            }
            else if((await _repository.CountActiveAsync(user.UserId, ct)) == _maxTasks)
            {
                throw new TaskCountLimitException((int)_maxTasks);
            }
            else if (IsDublicate(user,task,ct))
            {
                throw new DublicateTaskException(name);
            }
            await _repository.AddAsync(task,ct);
            return task;
        }

        public async Task DeleteAsync(Guid id,CancellationToken ct)
        {
            await _repository.DeleteAsync(id,ct);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userid,CancellationToken ct)
        {
            return await _repository.GetActiveByUserIdAsync(userid,ct);
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userid,CancellationToken ct)
        {
            return await _repository.GetAllByUserIdAsync(userid, ct);
        }
        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, string namePrefix, CancellationToken ct)
        {
            return await _repository.FindAsync(userId, item => item.Name.StartsWith(namePrefix),ct);
        }

        public async Task MarkCompletedAsync(Guid id, CancellationToken ct)
        {
            ToDoItem? item = await _repository.GetAsync(id, ct);
            if (item != null)
            {
                await _repository.UpdateAsync(item,ct);
            }
        }
        public void SetMaxTaskLength()
        {
            Console.WriteLine("Введите максимально допустимую длину задачи(1-100):");
            int number = 0;
            while (_maxTaskLength == null)
            {
                try
                {
                    number = ParseAndValidateInt(Console.ReadLine(), 1, 100);
                }
                catch (ArgumentException ArEx) 
                {
                    Console.WriteLine(ArEx.Message);
                    continue;
                }
                _maxTaskLength = number;
                Console.WriteLine($"Максимальная длина задачи установлена:{_maxTaskLength}");
            }
        }

        public void SetMaxTask()
        {
            Console.WriteLine("Введите максимальное количество задач(1-100):");
            int number = 0;
            while (_maxTasks == null)
            {
                try
                {
                    number = ParseAndValidateInt(Console.ReadLine(), 1, 100);
                }
                catch (ArgumentException ArEx) 
                {
                    Console.WriteLine(ArEx.Message);
                    continue; 
                }
                _maxTasks = number;
                Console.WriteLine($"Максимальное количество задач установлено:{_maxTasks}");
            }
        }
        private bool IsDublicate(ToDoUser user,ToDoItem task, CancellationToken ct)
        {
            return _repository.ExistsByNameAsync(user.UserId, task.Name, ct).Result;
        }
        int ParseAndValidateInt(string? str, int min, int max)
        {
            ValidateString(str);
            if (int.TryParse(str, out int number))
            {
                if (number < min || number > max)
                    throw new ArgumentException($"Значение вне диапазона({min}-{max})!");
                return number;
            }
            else
                throw new ArgumentException("Допустимы только цифры\\числа!");
        }
        void ValidateString(string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Строка не должна быть Null или пустой");
        }
    }
}
