using ConsoleApp1.DataAccess;
using ConsoleApp1.Exceptions;
using ConsoleApp1.Infrastructure.DataAccess;

namespace ConsoleApp1.Entities
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
        public ToDoItem Add(ToDoUser user, string name)
        {
            ValidateString(name);
            ToDoItem Task = new ToDoItem(user, name);
            if(Task.Name.Length > _maxTaskLength)
            {
                throw new TaskLenghtLimitException(Task.Name.Length, (int)_maxTaskLength);
            }
            else if(_repository.CountActive(user.UserId) == _maxTasks)
            {
                throw new TaskCountLimitException((int)_maxTasks);
            }
            else if (IsDublicate(user,Task))
            {
                throw new DublicateTaskException(name);
            }
            _repository.Add(Task);
            return Task;
        }

        public void Delete(Guid id)
        {
            _repository.Delete(id);
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userid)
        {
            return _repository.GetActiveByUserId(userid);
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userid)
        {
            return _repository.GetAllByUserId(userid);
        }
        public IReadOnlyList<ToDoItem> Find(Guid userId, string namePrefix)
        {
            return _repository.Find(userId, item => item.Name.StartsWith(namePrefix));
        }

        public void MarkCompleted(Guid id)
        {
            ToDoItem? item = _repository.Get(id);
            if (item != null)
            {
                _repository.Update(item);
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
        private bool IsDublicate(ToDoUser user,ToDoItem task)
        {
            if (_repository.ExistsByName(user.UserId, task.Name))
                return true;
            else
                return false;
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
