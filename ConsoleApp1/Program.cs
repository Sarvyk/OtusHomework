using ConsoleApp1.Classes;
using System.Reflection;

namespace ConsoleApp1
{
    internal class Program
    {
        static string? UserName = "Гость";
        static bool Exit = false;
        static List<string> Tasks = new List<string>();
        static int CountTasks;
        static int MaxTaskLength;
        static void Main(string[] args)
        {
            Console.WriteLine($"Добрый день {UserName}!");
            HelpCommand();
            while (!Exit)
            {
                try
                {
                    if (CountTasks == 0)
                        CountTasks = SetMaxTask();
                    if (MaxTaskLength == 0)
                        MaxTaskLength = SetMaxTaskLength();
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
                            EchoCommand(input.Remove(0, 5).Trim());
                            break;
                        case "/addtask":
                            AddtaskCommand();
                            break;
                        case "/showtask":
                            ShowTaskCommand();
                            break;
                        case "/removetask":
                            RemoveTaskCommand();
                            break;
                        case "/exit":
                            ExitCommand();
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
            if (UserName != "Гость")
            {
                Console.WriteLine($"{UserName}, комманда /start уже была выполнена!");
                return;
            }
            Console.Write("Введите своё имя:");
            string? tmp = Console.ReadLine();
            ValidateString(tmp);
            UserName = tmp;
            Console.WriteLine($"Добрый день {UserName}!");
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
                if (TaskName.Length > MaxTaskLength)
                {
                    throw new TaskLenghtLimitException(TaskName.Length, MaxTaskLength);
                }
                else if (Tasks.Contains(TaskName))
                {
                    throw new DublicateTaskException(TaskName);
                }
                else
                {
                    Tasks.Add(TaskName);
                    Console.WriteLine("Задача успешно добавлена!");
                    return;
                }
            }
        }
        static bool ShowTaskCommand()
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
                Console.WriteLine($"{i + 1}){Tasks[i]}");
                i++;
            }
            return true;
        }
        static void RemoveTaskCommand()
        {
            if (!ShowTaskCommand())
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
        static void HelpCommand()
        {
            Console.WriteLine($"{UserName}, используйте следующий список команд для работы:\r\n" +
                "/start - для запуска\r\n" +
                "/help - вывод помощи\r\n" +
                "/info - вывод информации по программе\r\n" +
                "/echo - вывод сообщения\r\n" +
                "/addtask - добавить задачу\r\n" +
                "/showtask - показать список задач\r\n" +
                "/removetask - удалить задачу из списка\r\n" +
                "/exit - выход из программы");
        }
        static void InfoCommand()
        {
            Console.WriteLine($"{UserName}, текущая версия программы {Assembly.GetEntryAssembly().GetName().Version.ToString()}. Дата создания {DateTime.Now.ToString("d")}");
        }
        static void EchoCommand(string command)
        {
            if (UserName == "Гость")
            {
                Console.WriteLine($"{UserName}, сначала введите /start, прежде чем использовать /echo!");
            }
            else
            {
                Console.WriteLine(command);
            }
        }
        static void ExitCommand()
        {
            Console.WriteLine($"До свидания, {UserName}!");
            Exit = true;
        }
    }
}