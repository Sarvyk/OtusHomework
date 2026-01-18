using ConsoleApp1.Classes;
using Otus.ToDoList.ConsoleBot;
using System.Reflection;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var botClient = new ConsoleBotClient();
                var handler = new UpdateHandler(new UserService(), new ToDoService());
                botClient.StartReceiving(handler);
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
            }

        }
    }
}