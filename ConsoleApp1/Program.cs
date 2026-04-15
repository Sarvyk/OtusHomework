using ConsoleApp1.Classes;
using ConsoleApp1.Core.Scenarios;
using ConsoleApp1.Core.Scenarios.Interfaces;
using ConsoleApp1.Core.Services;
using ConsoleApp1.Helpers;
using ConsoleApp1.Infrastructure.DataAccess;
using DotNetEnv;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1
{
    internal class Program
    {
        private static readonly string _token;
        const string storagePath = "UserStorage";
        const string connectionString = @"host=127.0.0.1; port=5432; Database=ToDoList; Username=postgres; password=123; Timeout=10; sslmode=prefer;";
        static Program()
        {
            Env.Load();
            _token = Env.GetString("API_TOKEN");
        }
        static async Task Main(string[] args)
        {
            FileLinkIndex.Initialize(storagePath);
            var botClient = new TelegramBotClient(_token);
            var userRepository = new SqlUserRepository(new DataContextFactory(connectionString));
            var serviceRepository = new SqlToDoRepository(new DataContextFactory(connectionString));
            var listRepository =new SqlToDoListRepository(new DataContextFactory(connectionString));
            var userSerivce = new UserService(userRepository);
            var toDoService = new ToDoService(serviceRepository);
            var toDoListService = new ToDoListService(listRepository);
            var scenarios = new List<IScenario>()
            {
                new AddTaskScenario(userSerivce, toDoListService, toDoService),
                new AddListScenario(userSerivce, toDoListService),
                new DeleteListScenario(userSerivce, toDoListService, toDoService),
                new DeleteTaskScenario(toDoService)
            };
            var handler = new UpdateHandler(userSerivce, toDoService, toDoListService, scenarios, new InMemoryScenarioContextRepository());
            var cts = new CancellationTokenSource();
            //botClient.DeleteWebhook(true);
            botClient.StartReceiving(handler, cancellationToken: cts.Token);
            await SetCommantList(botClient);
            var myBot = await botClient.GetMe();
            Console.WriteLine($"-------------Бот \"{myBot.FirstName}\" работает.-------------");
            await KeyCheck(myBot, cts);
            await Task.Delay(-1);
        }

        private static async Task KeyCheck(User bot, CancellationTokenSource cts)
        {
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.A)
                {
                    Console.WriteLine("Асинхронные операции отменены.");
                    cts.Cancel();
                    break;
                }
                else
                {
                    Console.WriteLine($@"------------Информация о боте------------
Никнейм:{bot.Username}
{bot.FirstName}
{bot.LastName}");
                }
            }
        }
        private static async Task SetCommantList(ITelegramBotClient botClient)
        {
            await botClient.SetMyCommands(new List<BotCommand>()
            {
                new BotCommand("help","вызов помощи"),
                new BotCommand("info","информация по приложению"),
                new BotCommand("addtask","`/addtask название`Добавить задачу."),
                new BotCommand("show","показывает список листов задач"),
                new BotCommand("report","статистика по задачам"),
                new BotCommand("find","`/find назв` выводит список задач, начиная с введённых символов.")
            });
        }
    }
}