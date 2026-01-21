using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace ConsoleApp1.Classes
{
    internal class ToDoService : IToDoService
    {
        private readonly List<ToDoItem> _tasks = new List<ToDoItem>();
        private int? MaxTasks;
        private int? MaxTaskLength;
        public ToDoService()
        {
            SetMaxTask();
            SetMaxTaskLength();
        }
        public ToDoItem Add(ToDoUser user, string name)
        {
            ValidateString(name);
            ToDoItem Task = new ToDoItem(user, name);
            if(Task.Name.Length >MaxTaskLength)
            {
                throw new TaskLenghtLimitException(Task.Name.Length, (int)MaxTaskLength);
            }
            else if(_tasks.Count == MaxTasks)
            {
                throw new TaskCountLimitException((int)MaxTasks);
            }
            else if (IsDublicate(Task))
            {
                throw new DublicateTaskException(name);
            }
            _tasks.Add(Task);
            return Task;
        }

        public void Delete(Guid id)
        {
            if (_tasks.RemoveAll(n => n.id == id) == 0)
                throw new ArgumentException("Такой задачи нет, либо список пуст");
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userid)
        {
            IReadOnlyList<ToDoItem> ActiveItems = _tasks.FindAll(n=>n.State == ToDoItemState.Active && n.User.UserId == userid);
            return ActiveItems;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userid)
        {
            IReadOnlyList<ToDoItem> AllItems = _tasks.FindAll(n=>n.User.UserId == userid);
            return AllItems;
        }

        public void MarkCompleted(Guid id)
        {
            foreach(ToDoItem Task in _tasks)
            {
                if(Task.id == id)
                {
                    Task.State = ToDoItemState.Completed;
                    break;
                }
            }
        }
        public void SetMaxTaskLength()
        {
            Console.WriteLine("Введите максимально допустимую длину задачи(1-100):");
            int number = 0;
            while (MaxTaskLength == null)
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
                MaxTaskLength = number;
                Console.WriteLine($"Максимальная длина задачи установлена:{MaxTaskLength}");
            }
        }

        public void SetMaxTask()
        {
            Console.WriteLine("Введите максимальное количество задач(1-100):");
            int number = 0;
            while (MaxTasks == null)
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
                MaxTasks = number;
                Console.WriteLine($"Максимальное количество задач установлено:{MaxTasks}");
            }
        }
        private bool IsDublicate(ToDoItem task)
        {
            foreach (ToDoItem Task in _tasks)
            {
                if (Task.Name == task.Name)
                    return true;
            }
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
