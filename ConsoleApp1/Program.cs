using ConsoleApp1.Classes;
using ConsoleApp1.Entities;
using ConsoleApp1.Infrastructure.DataAccess;
using ConsoleApp1.Services;
using Otus.ToDoList.ConsoleBot;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var botClient = new ConsoleBotClient();
                var userMemory = new InMemoryUserRepository();
                var serviceMemory = new InMemoryToDoRepository();
                var handler = new UpdateHandler(new UserService(userMemory), new ToDoService(serviceMemory));
                botClient.StartReceiving(handler);
            }
            catch (Exception ex)
            { 
                Console.WriteLine(ex.Message);
            }

        }
    }
}