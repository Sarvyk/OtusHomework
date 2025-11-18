using System.Reflection;
using System.Runtime.CompilerServices;

namespace ConsoleApp1
{
    internal class Program
    {
        static string? UserName = "Гость";
        static void Main(string[] args)
        {
            Console.WriteLine($"Добрый день {UserName}!");
            HelpCommand();
            while (true)
            {
                Console.Write("Ответ:");
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
        static void HelpCommand()
        {
            Console.WriteLine($"{UserName}, используйте следующий список команд для работы:\r\n" +
                "/start - для запуска\r\n" +
                "/help - вывод помощи\r\n" +
                "/info - вывод информации по программе\r\n" +
                "/echo - вывод сообщения\r\n" +
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
            Environment.Exit(0);
        }
    }
}