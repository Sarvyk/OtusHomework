using ConsoleApp1.Classes;
using System.Reflection;

namespace ConsoleApp1
{
    internal class Program
    {
        static ToDoUser User;
        static List<ToDoItem> Tasks = new List<ToDoItem>();
        static bool Exit = false;
        static int CountTasks;
        static int MaxTaskLength;
        static void Main(string[] args)
        {
            Console.WriteLine($"Добрый день, {GetUserName()}!");
            HelpCommand();
            while (!Exit)
            {
                try
                {
                    Console.Write("Запрос:");
                    string? input = Console.ReadLine();
                    ValidateString(input);
                    switch (input)
                    {
                        case "/start":
                            StartCommand();
                            break;
                        case "/help":
                            HelpCommand();
                            break;
                        case "/info":
                            InfoCommand();
                            break;
                        case string a when a.IndexOf("/echo") == 0:
                            if (IsAuth())
                                EchoCommand(input.Remove(0, 5).Trim());
                            break;
                        case "/addtask":
                            if (IsAuth())
                                AddtaskCommand();
                            break;
                        case "/showtask":
                            if (IsAuth())
                                ShowTaskCommand();
                            break;
                        case "/showalltask":
                            if (IsAuth())
                                ShowTaskCommand(true);
                            break;
                        case string a when a.IndexOf("/completetask") == 0:
                            if (IsAuth())
                                CompliteTaskCommand(input.Remove(0,13).Trim());
                                break;
                        case "/removetask":
                            if (IsAuth())
                                RemoveTaskCommand();
                            break;
                        case "/exit":
                            ExitCommand();
                            break;
                        default:
                            Console.WriteLine("Такой команды не существует!");
                            break;
                    }
                }
                catch (TaskCountLimitException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (TaskLenghtLimitException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (DublicateTaskException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла непредвиденная ошибка:{ex.Message}\r\n{ex.StackTrace}\r\n{ex.InnerException}");
                }
            }
        }

        private static bool IsAuth()
        {
            if (User == null)
            {
                Console.WriteLine("Представьтесь, прежде чем использовать эту команду(/start)");
                return false;
            }
            else
                return true;
        }
        private static int SetMaxTaskLength()
        {
            Console.WriteLine("Введите максимально допустимую длину задачи(1-100):");
            int number;
            while (true)
            {
                Console.Write("Ответ:");
                number = ParseAndValidateInt(Console.ReadLine(), 1, 100);
                Console.WriteLine($"Максимальная длина задачи установлена:{number}");
                return number;
            }
        }

        private static int SetMaxTask()
        {
            Console.WriteLine("Введите максимальное количество задач(1-100):");
            int number;
            while (true)
            {
                Console.Write("Ответ:");
                number = ParseAndValidateInt(Console.ReadLine(), 1, 100);
                Console.WriteLine($"Максимальное количество задач установлено:{number}");
                return number;
            }
        }
        static int ParseAndValidateInt(string? str, int min, int max)
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
        static void ValidateString(string? str)
        {
            if (string.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Строка не должна быть Null или пустой");
        }
        static void StartCommand()
        {
            if (User != null)
            {
                Console.WriteLine($"{User.TelegramUserName}, комманда /start уже была выполнена!");
                return;
            }
            Console.Write("Введите своё имя:");
            string? tmp = Console.ReadLine();
            ValidateString(tmp);
            User = new ToDoUser(tmp);
            Console.WriteLine($"Добрый день {User.TelegramUserName}!");
            CountTasks = SetMaxTask();
            MaxTaskLength = SetMaxTaskLength();
        }
        static void AddtaskCommand()
        {
            if (Tasks.Count == CountTasks)
                throw new TaskCountLimitException(CountTasks);
            while (true)
            {
                Console.Write("Пожалуйста, введите описание задачи:");
                string TaskName = Console.ReadLine();
                ValidateString(TaskName);
                ToDoItem Task = new ToDoItem(User, TaskName);
                if (Task.Name.Length > MaxTaskLength)
                {
                    throw new TaskLenghtLimitException(Task.Name.Length, MaxTaskLength);
                }
                else if (IsDublicate(Task))
                {
                    throw new DublicateTaskException(TaskName);
                }
                else
                {
                    Tasks.Add(Task);
                    Console.WriteLine("Задача успешно добавлена!");
                    return;
                }
            }
        }
        static bool IsDublicate(ToDoItem task)
        {
            foreach(ToDoItem Task in Tasks)
            {
                if (Task.Name == task.Name)
                    return true;
            }
            return false;
        }
        static bool ShowTaskCommand(bool allTasks = false)
        {
            if (Tasks.Count == 0)
            {
                Console.WriteLine("Список задач пуст!");
                return false;
            }
            Console.WriteLine("Список задач:");
            int i = 0;
            while (i < Tasks.Count)
            {
                if (allTasks)
                {
                    Console.WriteLine($"{i + 1})({Tasks[i].State}){Tasks[i].Name} - Время создания:{Tasks[i].CreatedAt} - Пользователь создавший задачу:{Tasks[i].User.TelegramUserName} - ID:{Tasks[i].id} Дата изменения статуса:{Tasks[i].StateChangedAt}");
                }
                else
                {
                    if (Tasks[i].State == ToDoItemState.Actrive)
                        Console.WriteLine($"{i + 1}){Tasks[i].Name} - Время создания:{Tasks[i].CreatedAt} - ID:{Tasks[i].id}");
                }
                i++;
            }
            return true;
        }
        static void RemoveTaskCommand()
        {
            if (!ShowTaskCommand(true))
            {
                return;
            }
            Console.WriteLine("Какую из задач хотите удалить?");
            Console.Write("Ответ:");
            int i = 0;
            i = ParseAndValidateInt(Console.ReadLine(), 1, Tasks.Count);
            Tasks.RemoveAt(i - 1);
            Console.WriteLine("Задача успешно удалена.");
        }
        static void CompliteTaskCommand(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new ArgumentException();
            Guid tmpGuid = new Guid(id);
            foreach (ToDoItem Task in Tasks)
            {
                if (Task.id == tmpGuid)
                {
                    Task.State = ToDoItemState.Completed;
                    Console.WriteLine("Задача завершена");
                    return;
                }
            }
            Console.WriteLine("Задача НЕ найдена!!!");
        }
        static void HelpCommand()
        {
            Console.WriteLine($"{GetUserName()}, используйте следующий список команд для работы:\r\n" +
                "/start - для запуска\r\n" +
                "/help - вывод помощи\r\n" +
                "/info - вывод информации по программе\r\n" +
                "/echo - вывод сообщения\r\n" +
                "/addtask - добавить задачу\r\n" +
                "/showtask - показать список задач\r\n" +
                "/completetask - завершить задачу\r\n" +
                "/showalltask - показать все задачи\r\n"+
                "/removetask - удалить задачу из списка\r\n" +
                "/exit - выход из программы");
        }
        static string GetUserName()
        {
            if (User == null)
                return "Гость";
            else
                return User.TelegramUserName;
        }
        static void InfoCommand()
        {
            Console.WriteLine($"{GetUserName()}, текущая версия программы {Assembly.GetEntryAssembly().GetName().Version.ToString()}. Дата создания {DateTime.Now.ToString("d")}");
        }
        static void EchoCommand(string command)
        {
            Console.WriteLine(command);
        }
        static void ExitCommand()
        {
            Console.WriteLine($"До свидания, {GetUserName()}!");
            Exit = true;
        }
    }
}