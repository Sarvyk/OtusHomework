using ConsoleApp1.DataAccess;
using ConsoleApp1.Entities;
using ConsoleApp1.Exceptions;
using ConsoleApp1.Services;
using Sprache;
using System;
using System.Reflection;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


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
            try
            {
                switch (update.Message.Text)
                {
                    case "/start":
                        await StartCommand(botClient, update, ct);
                        await HelpCommand(botClient, update, ct);
                        break;
                    case "/help":
                        await HelpCommand(botClient, update, ct);
                        break;
                    case "/info":
                        await InfoCommand(botClient, update, ct);
                        break;
                    case string a when a.IndexOf("/addtask") == 0:
                        if (IsRegistered(botClient, update, ct))
                        {
                            await _toDoService.AddAsync(await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct), a.Replace("/addtask", "").Trim(), ct);
                            await botClient.SendMessage(update.Message.Chat, "Задача успешно добавлена", cancellationToken: ct);
                        }
                        break;
                    case string a when a.IndexOf("/completetask") == 0:
                        if (IsRegistered(botClient, update, ct))
                        {
                            Guid guid = new Guid();
                            if (!Guid.TryParse(a.Replace("/completetask", ""), out guid))
                                throw new ArgumentException("Такого id нет!");
                            await _toDoService.MarkCompletedAsync(guid, ct);
                            await botClient.SendMessage(update.Message.Chat, "Задача завершена", cancellationToken: ct);
                        }
                        break;
                    case string a when a.IndexOf("/removetask") == 0:
                        if (IsRegistered(botClient, update, ct))
                        {
                            Guid guid = new Guid();
                            if (!Guid.TryParse(a.Replace("/removetask", ""), out guid))
                                throw new ArgumentException("Такого id нет!");
                            await _toDoService.DeleteAsync(guid, ct);
                            await botClient.SendMessage(update.Message.Chat, "Задача успешно удалена", cancellationToken: ct);
                        }
                        break;
                    case "/showtasks":
                        if (IsRegistered(botClient, update, ct))
                        {
                            await ShowTasks(botClient, update, true, ct);
                        }
                        break;
                    case "/showalltasks":
                        if (IsRegistered(botClient, update, ct))
                        {
                            await ShowTasks(botClient, update, false, ct);
                        }
                        break;
                    case "/report":
                        if (IsRegistered(botClient, update, ct))
                        {
                            IToDoReportService report = new ToDoReportService(_toDoService);
                            var stat = (await report.GetUserStatsAsync((await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct)).UserId, ct));
                            await botClient.SendMessage(update.Message.Chat, $"Статистика по задачами на {stat.generatedAt}. Всего: {stat.total}; Завершённых: {stat.completed}; Активных: {stat.active}.", cancellationToken: ct);
                        }
                        break;
                    case string a when a.IndexOf("/find") == 0:
                        if (IsRegistered(botClient, update, ct))
                        {
                            await botClient.SendMessage(update.Message.Chat, await FindTasks(update, a.Replace("/find", "").Trim(), ct), cancellationToken: ct);
                        }
                        break;
                    default:
                        await botClient.SendMessage(update.Message.Chat, "Такой команды не существует!", cancellationToken: ct);
                        break;
                }
            }
            catch(ArgumentException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
            }
            catch(TaskCountLimitException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
    }
            catch(TaskLenghtLimitException ex)
            {
                await botClient.SendMessage(update.Message.Chat, ex.Message, cancellationToken: ct);
}
            catch(Exception ex)
            {
                await HandleErrorAsync(botClient, ex, HandleErrorSource.HandleUpdateError, ct);
            }
        }
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource handleError, CancellationToken ct)
        {
            Console.WriteLine(exception.Message);
        }
        private async Task ShowTasks(ITelegramBotClient botClient, Update update, bool isActive, CancellationToken ct)
        {
            Guid guid = (await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct)).UserId;
            List<ToDoItem> data = new List<ToDoItem>();
            string result = "\r\n";
            if(isActive)
                data = (await _toDoService.GetActiveByUserIdAsync(guid, ct)).ToList();
            else
                data = (await _toDoService.GetAllByUserIdAsync(guid, ct)).ToList();
            int i = 1;
            foreach(ToDoItem Task in data)
            {
                if(isActive)
                    result += $"{i++})ID:`{Task.id}`, Название:{Task.Name}, Дата создания:{Task.CreatedAt}\r\n";
                else
                    result += $"{i++})ID:`{Task.id}`, Название:{Task.Name}, Дата создания:{Task.CreatedAt}, Статус:{Task.State}, Изменение статуса:{Task.StateChangedAt}\r\n";
            }
            result = result.Remove(result.Length - 2);
            result = EscapeString(result);
            if (result == string.Empty)
                await botClient.SendMessage(update.Message.Chat, "Задач в списке нет", cancellationToken: ct, parseMode: ParseMode.MarkdownV2);
            else
                await botClient.SendMessage(update.Message.Chat, result, cancellationToken: ct, parseMode: ParseMode.MarkdownV2);
        }
        private string EscapeString(string str)
        {
            char[] esc = new char[] { '\\', '*', '_', '{', '}', '[', ']', '(', ')', '#', '+', '-', '.', '!' };
            char[] strChars = str.ToCharArray();
            StringBuilder sBResult = new StringBuilder();
            int i = 0;
            while (strChars.Length > i)
            {
                if (esc.Contains(strChars[i]))
                {
                    sBResult.Append("\\" + strChars[i++]);
                }
                else
                    sBResult.Append(strChars[i++]);
            }
            return sBResult.ToString();
        }
        private async Task<string> FindTasks(Update update, string namePrefix, CancellationToken ct)
        {
            Guid guid = (await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct)).UserId;
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
        private async Task StartCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            ToDoUser? User = _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct).Result;
            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(new List<KeyboardButton>
                {
                    new KeyboardButton("/showalltasks"),
                    new KeyboardButton("/showtasks"),
                    new KeyboardButton("/report")
                });
            if (User != null)
            {
                await botClient.SendMessage(update.Message.Chat, $"{User.TelegramUserName}, команда уже выполнена.", replyMarkup: keyboard, cancellationToken: ct);
            }
            else
            {
                User = await _userService.RegisterUserAsync(update.Message.From.Id, update.Message.From.Username ?? "", ct);
                await botClient.SendMessage(update.Message.Chat, $"{User.TelegramUserName}, добро пожаловать!", replyMarkup: keyboard, cancellationToken: ct);
            }
        }
        private bool IsRegistered(ITelegramBotClient bot,Update update,CancellationToken ct)
        {
            if (_userService.GetUserByTelegramUserIdAsync(update.Message.From.Id,ct).Result == null)
            {
                bot.SendMessage(update.Message.Chat, "Команда доступна только для зарегистрированных пользователей. /start Для запуска.", cancellationToken: ct);
                return false;
            }
            else
                return true;
        }
        private async Task HelpCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            if (await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct) != null)
                await botClient.SendMessage(update.Message.Chat, $"Используйте следующий список команд для работы:\r\n" +
                "/help - вывод помощи\r\n" +
                "/info - вывод информации по программе\r\n" +
                "/addtask [название] - добавить задачу\r\n" +
                "/showtasks - показать список задач\r\n" +
                "/showalltasks - показать все задачи\r\n" +
                "/completetask [id] - завершить задачу\r\n" +
                "/removetask [id] - удалить задачу из списка\r\n" +
                "/report - статистика по задачам\r\n" +
                "/find [значение] - выводит список задач, которые начинаются с определённого значения", cancellationToken: ct);
            else
                await botClient.SendMessage(update.Message.Chat, "Для не зарегестрированного пользователя доступны только команды /start", cancellationToken: ct);
        }
        private async Task InfoCommand(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            await botClient.SendMessage(update.Message.Chat, $"Текущая версия программы {Assembly.GetEntryAssembly().GetName().Version.ToString()}. Дата создания {DateTime.Now.ToString("d")}", cancellationToken: ct);
        }
    }
}