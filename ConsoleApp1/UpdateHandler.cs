using ConsoleApp1.DataAccess;
using ConsoleApp1.Entities;
using ConsoleApp1.Exceptions;
using ConsoleApp1.Services;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System.Reflection;


namespace ConsoleApp1.Classes
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            switch (update.Message.Text)
            {
                case "/start":
                    await botClient.SendMessage(update.Message.Chat, StartCommand(update, ct), ct);
                    await botClient.SendMessage(update.Message.Chat, $"{HelpCommand(update, ct)}", ct);
                    break;
                case "/help":
                    await botClient.SendMessage(update.Message.Chat, $"{HelpCommand(update, ct)}", ct);
                    break;
                case "/info":
                    await botClient.SendMessage(update.Message.Chat, $"{InfoCommand()}", ct);
                    break;
                case string a when a.IndexOf("/addtask") == 0:
                    if (IsRegistered(botClient, update, ct))
                    {
                        await _toDoService.AddAsync(await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct), a.Replace("/addtask", "").Trim(), ct);
                        await botClient.SendMessage(update.Message.Chat, "Задача успешно добавлена", ct);
                    }
                    break;
                case string a when a.IndexOf("/completetask") == 0:
                    if (IsRegistered(botClient, update, ct))
                    {
                        Guid guid = new Guid();
                        if (!Guid.TryParse(a.Replace("/completetask", ""), out guid))
                            throw new ArgumentException("Такого id нет!");
                        await _toDoService.MarkCompletedAsync(guid, ct);
                        await botClient.SendMessage(update.Message.Chat, "Задача завершена", ct);
                    }
                    break;
                case string a when a.IndexOf("/removetask") == 0:
                    if (IsRegistered(botClient, update, ct))
                    {
                        Guid guid = new Guid();
                        if (!Guid.TryParse(a.Replace("/removetask", ""), out guid))
                            throw new ArgumentException("Такого id нет!");
                        await _toDoService.DeleteAsync(guid, ct);
                        await botClient.SendMessage(update.Message.Chat, "Задача успешно удалена", ct);
                    }
                    break;
                case "/showtask":
                    if (IsRegistered(botClient, update, ct))
                    {
                        await botClient.SendMessage(update.Message.Chat, ShowTasks(botClient, update, true, ct), ct);
                    }
                    break;
                case "/showalltask":
                    if (IsRegistered(botClient, update, ct))
                    {
                       await botClient.SendMessage(update.Message.Chat, ShowTasks(botClient, update, false, ct), ct);
                    }
                    break;
                case "/report":
                    if (IsRegistered(botClient, update, ct))
                    {
                        IToDoReportService report = new ToDoReportService(_toDoService);
                        var stat = (await report.GetUserStatsAsync(_userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct).Result.UserId, ct));
                        await botClient.SendMessage(update.Message.Chat, $"Статистика по задачами на {stat.generatedAt}. Всего: {stat.total}; Завершённых: {stat.completed}; Активных: {stat.active}.", ct);
                    }
                    break;
                case string a when a.IndexOf("/find") == 0:
                    if (IsRegistered(botClient, update, ct))
                    {
                        await botClient.SendMessage(update.Message.Chat, FindTasks(update, a.Replace("/find", "").Trim(), ct), ct);
                    }
                    break;
                default:
                    await botClient.SendMessage(update.Message.Chat, "Такой команды не существует!", ct);
                    break;
            }
        }
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken ct)
        {
            Console.WriteLine(exception.Message);
        }
        private string ShowTasks(ITelegramBotClient bot, Update update, bool isActive, CancellationToken ct)
        {
            Guid guid = _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id,ct).Result.UserId;
            List<ToDoItem> data = new List<ToDoItem>();
            string result = "\r\n";
            if(isActive)
                data = _toDoService.GetActiveByUserIdAsync(guid,ct).Result.ToList();
            else 
                data = _toDoService.GetAllByUserIdAsync(guid,ct).Result.ToList();
            int i = 1;
            foreach(ToDoItem Task in data)
            {
                if(isActive)
                    result += $"{i++})ID:{Task.id}, Название:{Task.Name}, Дата создания:{Task.CreatedAt}\r\n";
                else 
                    result += $"{i++})ID:{Task.id}, Название:{Task.Name}, Дата создания:{Task.CreatedAt}, Статус:{Task.State}, Изменение статуса:{Task.StateChangedAt}\r\n";
            }
            result = result.Remove(result.Length - 2);
            if (result == string.Empty)
                return "Задач в списке нет";
            else
                return result;
        }
        private string FindTasks(Update update, string namePrefix, CancellationToken ct)
        {
            Guid guid = _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id,ct).Result.UserId;
            var tasks = _toDoService.FindAsync(guid, namePrefix, ct).Result;
            string result = string.Empty;
            int i = 1;
            foreach (ToDoItem Task in tasks)
            {
                result += $"{i++})ID:{Task.id}, Название:{Task.Name}, Дата создания:{Task.CreatedAt}\r\n";
            }
            if(result == string.Empty)
                result = "Задач в списке нет.";
            return result;
        }
        private string StartCommand(Update update, CancellationToken ct)
        {
            ToDoUser? User = _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct).Result;
            if (User != null)
            {
                return $"{User.TelegramUserName}, команда уже выполнена.";
            }
            else
            {
                User = _userService.RegisterUserAsync(update.Message.From.Id, update.Message.From.Username, ct).Result;
                return $"{User.TelegramUserName}, добро пожаловать!";
            }
        }
        private bool IsRegistered(ITelegramBotClient bot,Update update,CancellationToken ct)
        {
            if (_userService.GetUserByTelegramUserIdAsync(update.Message.From.Id,ct).Result == null)
            {
                bot.SendMessage(update.Message.Chat, "Команда доступна только для зарегистрированных пользователей. /start Для запуска.",ct);
                return false;
            }
            else
                return true;
        }
        private string HelpCommand(Update update, CancellationToken ct)
        {
            if (_userService.GetUserByTelegramUserIdAsync(update.Message.From.Id,ct).Result != null)
                return $"Используйте следующий список команд для работы:\r\n" +
                "/help - вывод помощи\r\n" +
                "/info - вывод информации по программе\r\n" +
                "/addtask [название] - добавить задачу\r\n" +
                "/showtask - показать список задач\r\n" +
                "/showalltask - показать все задачи\r\n" +
                "/completetask [id] - завершить задачу\r\n" +
                "/removetask [id] - удалить задачу из списка\r\n" +
                "/report - статистика по задачам\r\n"+
                "/find [значение] - выводит список задач, которые начинаются с определённого значения";
            else
                return "Для не зарегестрированного пользователя доступны только команды /start, /help и /info";
        }
        private string InfoCommand()
        {
            return $"Текущая версия программы {Assembly.GetEntryAssembly().GetName().Version.ToString()}. Дата создания {DateTime.Now.ToString("d")}";
        }
    }
}