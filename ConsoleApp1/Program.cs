using System.Reflection;
using System.Runtime.CompilerServices;

namespace ConsoleApp1
{
    internal class Program
    {
        static string? UserName = "Гость";
        static bool Exit = false;
        static List<string> Tasks = new List<string>();
        static void Main(string[] args)
        {
            Console.WriteLine($"Добрый день {UserName}!");
            HelpCommand();
            while (!Exit)
            {
                Console.Write("Запрос:");
                string? input = Console.ReadLine();
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
                        EchoCommand(input.Remove(0,5).Trim());
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
                    default:
                        Console.WriteLine("Комманда не распознана!");
                        HelpCommand();
                        break;
                }
            }
        }
        static void StartCommand()
        {
            if (UserName != "Гость")
            {
                Console.WriteLine($"{UserName}, комманда /start уже была выполнена!");
                return;
            }
            Console.Write("Введите своё имя:");
            UserName = Console.ReadLine();
            Console.WriteLine($"Добрый день {UserName}!");
        }
        static void AddtaskCommand()
        {
            while (true)
            {
                Console.Write("Пожалуйста, введите описание задачи:");
                string TaskName = Console.ReadLine();
                if (TaskName == string.Empty)
                {
                    Console.WriteLine("Вы ничего не ввели");
                    continue;
                }else
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
            while (i<Tasks.Count)
            {
                Console.WriteLine($"{i+1}){Tasks[i]}");
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
            int i =0;
            do
            {
                if (int.TryParse(Console.ReadLine(), out i))
                {
                    try 
                    { 
                        Tasks.RemoveAt(i-1);
                        Console.WriteLine("Задача успешно удалена.");
                        return;
                    }catch(ArgumentOutOfRangeException ex)
                    {
                        Console.WriteLine("Укажите номер задачи из списка!");
                        Console.Write("Ответ:");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine("Допустимы только цифры\\числа!!!");
                    Console.Write("Ответ:");
                }
            } while (true);
        }
        static void HelpCommand()
        {
            Console.WriteLine($"{UserName}, используйте следующий список команд для работы:\r\n" +
                "/start - для запуска\r\n" +
                "/help - вывод помощи\r\n" +
                "/info - вывод информации по программе\r\n" +
                "/echo - вывод сообщения\r\n" +
                "/addtask - добавить задачу\r\n"+
                "/showtask - показать список задач\r\n"+
                "/removetask - удалить задачу из списка\r\n"+
                "/exit - выход из программы");
        }
        static void InfoCommand()
        {
            Console.WriteLine($"{UserName}, текущая версия программы {Assembly.GetEntryAssembly().GetName().Version.ToString()}. Дата создания {DateTime.Now.ToString("d")}");
        }
        static void EchoCommand(string command)
        {
            if(UserName == "Гость")
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