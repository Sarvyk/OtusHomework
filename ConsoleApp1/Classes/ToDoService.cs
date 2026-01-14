using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Classes
{
    internal class ToDoService : IToDoService
    {
        private List<ToDoItem> Tasks = new List<ToDoItem>();
        int? MaxTasks;
        int? MaxTaskLength;
        public ToDoItem Add(ToDoUser user, string name)
        {
            ValidateString(name);
            ToDoItem Task = new ToDoItem(user, name);
            if(Task.Name.Length >MaxTaskLength)
            {
                throw new TaskLenghtLimitException(Task.Name.Length, (int)MaxTaskLength);
            }
            else if(Tasks.Count == MaxTasks)
            {
                throw new TaskCountLimitException((int)MaxTasks);
            }
            else if (IsDublicate(Task))
            {
                throw new DublicateTaskException(name);
            }
            Tasks.Add(Task);
            return Task;
        }

        public void Delete(Guid id)
        {
            if (Tasks.RemoveAll(n => n.id == id) == 0)
                throw new ArgumentException("Такой задачи нет, либо список пуст");
        }

        public IReadOnlyList<ToDoItem> GetActiveByUserId(Guid userId)
        {
            IReadOnlyList<ToDoItem> ActiveItems = Tasks.FindAll(n=>n.State == ToDoItemState.Active);
            return ActiveItems;
        }

        public IReadOnlyList<ToDoItem> GetAllByUserId(Guid userid)
        {
            IReadOnlyList<ToDoItem> AllItems = Tasks;
            return AllItems;
        }

        public void MarkCompleted(Guid id)
        {
            foreach(ToDoItem Task in Tasks)
            {
                if(Task.id == id)
                {
                    Task.State = ToDoItemState.Completed;
                    break;
                }
            }
        }
        public Guid GetTaskByNumber(string str)
        {
            int num = ParseAndValidateInt(str, 0, Tasks.Count) -1;
            return Tasks[num].id;
        }
        public void SetMaxTaskLength(ITelegramBotClient bot, Update update)
        {
            bot.SendMessage(update.Message.Chat,"Введите максимально допустимую длину задачи(1-100):");
            int number = 0;
            while (MaxTaskLength == null)
            {
                try
                {
                    number = ParseAndValidateInt(Console.ReadLine(), 1, 100);
                }
                catch (ArgumentException ArEx) 
                {
                    bot.SendMessage(update.Message.Chat, ArEx.Message);
                    continue;
                }
                MaxTaskLength = number;
                bot.SendMessage(update.Message.Chat, $"Максимальная длина задачи установлена:{MaxTaskLength}");
            }
        }

        public void SetMaxTask(ITelegramBotClient bot, Update update)
        {
            bot.SendMessage(update.Message.Chat, "Введите максимальное количество задач(1-100):");
            int number = 0;
            while (MaxTasks == null)
            {
                try
                {
                    number = ParseAndValidateInt(Console.ReadLine(), 1, 100);
                }
                catch (ArgumentException ArEx) 
                {
                    bot.SendMessage(update.Message.Chat, ArEx.Message);
                    continue; 
                }
                MaxTasks = number;
                bot.SendMessage(update.Message.Chat, $"Максимальное количество задач установлено:{MaxTasks}");
            }
        }
        private bool IsDublicate(ToDoItem task)
        {
            foreach (ToDoItem Task in Tasks)
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
