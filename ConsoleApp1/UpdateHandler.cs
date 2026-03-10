using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Exceptions;
using ConsoleApp1.Core.Interfaces.DataAccess;
using ConsoleApp1.Core.Scenarios;
using ConsoleApp1.Core.Scenarios.Enums;
using ConsoleApp1.Core.Scenarios.Interfaces;
using ConsoleApp1.Core.Services;
using ConsoleApp1.DTO;
using ConsoleApp1.Helpers;
using System.Collections;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace ConsoleApp1.Classes
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        private readonly IToDoListService _toDoListService;
        private readonly IEnumerable _scenarios;
        private readonly IScenarioContextRepository _scenarioContextRepository;
        public UpdateHandler(IUserService userService, IToDoService toDoService, IToDoListService toDoListService, IEnumerable scenarios, IScenarioContextRepository contextRepository)
        {
            _userService = userService;
            _toDoService = toDoService;
            _scenarios = scenarios;
            _scenarioContextRepository = contextRepository;
            _toDoListService = toDoListService;
        }
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.CallbackQuery:
                        if(await IsRegistered(botClient, update.CallbackQuery.Message, ct))
                            await HandleCallBack(botClient, update, ct);
                        break;
                    case UpdateType.Message:
                        await HandleMessage(botClient, update, ct);
                        break;
                    default:
                        await botClient.SendMessage(update.Message.Chat, "Такой формат пока не поддерживается!", replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                        return;
                }
            }
            catch (ArgumentException ex)
            {
                await botClient.SendMessage((update.Message != null)?update.Message.Chat : update.CallbackQuery.Message.Chat, ex.Message, replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                await _scenarioContextRepository.ResetContext((update.Message != null) ? update.Message.From.Id : update.CallbackQuery.From.Id, ct);
            }
            catch (TaskCountLimitException ex)
            {
                await botClient.SendMessage((update.Message != null) ? update.Message.Chat : update.CallbackQuery.Message.Chat, ex.Message, replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                await _scenarioContextRepository.ResetContext((update.Message != null) ? update.Message.From.Id : update.CallbackQuery.From.Id, ct);
            }
            catch (TaskLenghtLimitException ex)
            {
                await botClient.SendMessage((update.Message != null)?update.Message.Chat : update.CallbackQuery.Message.Chat, ex.Message, replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                await _scenarioContextRepository.ResetContext((update.Message != null) ? update.Message.From.Id : update.CallbackQuery.From.Id, ct);
            }
            catch(DublicateListException ex)
            {
                await botClient.SendMessage((update.Message != null) ? update.Message.Chat : update.CallbackQuery.Message.Chat, ex.Message, replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                await _scenarioContextRepository.ResetContext((update.Message != null) ? update.Message.From.Id : update.CallbackQuery.From.Id, ct);
            }
            catch (DublicateTaskException ex)
            {
                await botClient.SendMessage((update.Message != null)?update.Message.Chat : update.CallbackQuery.Message.Chat, ex.Message, replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                await _scenarioContextRepository.ResetContext((update.Message != null) ? update.Message.From.Id : update.CallbackQuery.From.Id, ct);
            }
            catch (Exception ex)
            {
                await HandleErrorAsync(botClient, ex, HandleErrorSource.HandleUpdateError, ct);
                await _scenarioContextRepository.ResetContext((update.Message != null) ? update.Message.From.Id : update.CallbackQuery.From.Id, ct);
            }
        }
        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource handleError, CancellationToken ct)
        {
            Console.WriteLine(exception.Message);
        }
        private async Task HandleMessage(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            ScenarioContext? context;
            if (update.Message.Text.StartsWith("/cancel"))
            {
                await _scenarioContextRepository.ResetContext(update.Message.From.Id, ct);
                await botClient.SendMessage(update.Message.Chat, "Сценарий отменён.", replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                return;
            }
            context = await _scenarioContextRepository.GetContext(update.Message.From.Id, ct);
            if (context != null)
            {
                await ProcessScenario(botClient, context, update.Message.From, update.Message, ct);
                return;
            }
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
                case "/addtask":
                    if (await IsRegistered(botClient, update.Message, ct))
                    {
                        context = new ScenarioContext(ScenarioType.AddTask);
                        await _scenarioContextRepository.SetContext(update.Message.From.Id, context, ct);
                        await ProcessScenario(botClient, context, update.Message.From, update.Message, ct);
                    }
                    break;
                case string a when a.IndexOf("/completetask") == 0:
                    if (await IsRegistered(botClient, update.Message, ct))
                    {
                        Guid guid = new Guid();
                        if (!Guid.TryParse(a.Replace("/completetask", ""), out guid))
                            throw new ArgumentException("Такого id нет!");
                        await _toDoService.MarkCompletedAsync(guid, ct);
                        await botClient.SendMessage(update.Message.Chat, "Задача завершена", cancellationToken: ct);
                    }
                    break;
                case string a when a.IndexOf("/removetask") == 0:
                    if (await IsRegistered(botClient, update.Message, ct))
                    {
                        Guid guid = new Guid();
                        if (!Guid.TryParse(a.Replace("/removetask", ""), out guid))
                            throw new ArgumentException("Такого id нет!");
                        await _toDoService.DeleteAsync(guid, ct);
                        await botClient.SendMessage(update.Message.Chat, "Задача успешно удалена", cancellationToken: ct);
                    }
                    break;
                case "/show":
                    if (await IsRegistered(botClient, update.Message, ct))
                    {
                        await ShowCommand(botClient, update, true, ct);
                    }
                    break;
                case "/report":
                    if (await IsRegistered(botClient, update.Message, ct))
                    {
                        IToDoReportService report = new ToDoReportService(_toDoService);
                        var stat = (await report.GetUserStatsAsync((await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct)).UserId, ct));
                        await botClient.SendMessage(update.Message.Chat, $"Статистика по задачами на {stat.generatedAt}. Всего: {stat.total}; Завершённых: {stat.completed}; Активных: {stat.active}.", cancellationToken: ct);
                    }
                    break;
                case string a when a.IndexOf("/find") == 0:
                    if (await IsRegistered(botClient, update.Message, ct))
                    {
                        await botClient.SendMessage(update.Message.Chat, await FindTasks(update, a.Replace("/find", "").Trim(), ct), cancellationToken: ct);
                    }
                    break;
                default:
                    await botClient.SendMessage(update.Message.Chat, "Такой команды не существует!", cancellationToken: ct);
                    break;
            }
        }
        private async Task HandleCallBack(ITelegramBotClient botClient, Update update, CancellationToken ct)
        {
            ScenarioContext? context = await _scenarioContextRepository.GetContext(update.CallbackQuery.From.Id, ct);
            if (context != null)
            {
                context.Data["Callback"] = update.CallbackQuery.Data;
                await ProcessScenario(botClient, context, update.CallbackQuery.From, update.CallbackQuery.Message, ct);
                return;
            }
            switch (update.CallbackQuery)
            {
                case CallbackQuery a when a.Data == "show":
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat, "Активных задач нет", cancellationToken: ct);
                    break;
                case CallbackQuery a when a.Data == "addlist":
                    context = new ScenarioContext(ScenarioType.AddList);
                    await _scenarioContextRepository.SetContext(update.CallbackQuery.From.Id, context, ct);
                    await ProcessScenario(botClient, context, update.CallbackQuery.From, update.CallbackQuery.Message, ct);
                    break;
                case CallbackQuery a when a.Data == "deletelist":
                    context = new ScenarioContext(ScenarioType.DeleteList);
                    await _scenarioContextRepository.SetContext(update.CallbackQuery.From.Id, context, ct);
                    await ProcessScenario(botClient, context, update.CallbackQuery.From, update.CallbackQuery.Message, ct);
                    break;
                case CallbackQuery a when a.Data.StartsWith("show"):
                    IReadOnlyList<ToDoItem> tasks = await _toDoService.GetByUserIdAndList((await _userService.GetUserByTelegramUserIdAsync(update.CallbackQuery.Message.From.Id, ct)).UserId, ToDoListCallbackDto.FromString(update.CallbackQuery.Data).ToDoListId, ct);
                    string result = string.Empty;
                    int i = 1;
                    foreach (ToDoItem task in tasks)
                    {
                        result += $"{i++})ID:`{task.id}`, Название:{task.Name}, Дата создания:{task.CreatedAt}, Дедлайн:{task.DeadLine}, Статус:{task.State}, Изменение статуса:{task.StateChangedAt}\r\n";
                    }
                    result = EscapeString(result);
                    if (result == "")
                        await botClient.SendMessage(update.CallbackQuery.Message.Chat, "Задач в списке нет", cancellationToken: ct, parseMode: ParseMode.MarkdownV2);
                    else
                        await botClient.SendMessage(update.CallbackQuery.Message.Chat, result, cancellationToken: ct, parseMode: ParseMode.MarkdownV2);
                    break;
            }
        }
        private async Task ProcessScenario(ITelegramBotClient botClient, ScenarioContext context, User user, Message msg, CancellationToken ct)
        {
            IScenario scenario = GetScenario(context.CurrentScenario);
            if (await scenario.HandleMessageAsync(botClient, context, msg, ct) == ScenarioResult.Completed)
                await _scenarioContextRepository.ResetContext(user.Id, ct);
        }

        private IScenario GetScenario(ScenarioType scenarioType)
        {
            foreach (IScenario scenario in _scenarios)
            {
                if(scenario.CanHandle(scenarioType))
                {
                    return scenario;
                }
            }
            throw new ArgumentException("Сценарий не найден");
        }

        private async Task ShowCommand(ITelegramBotClient botClient, Update update, bool isActive, CancellationToken ct)
        {
            List<InlineKeyboardButton[]> buttons = new List<InlineKeyboardButton[]>();
            buttons.Add(new[]
            {//добавляем первую кнопку массивом, чтобы были в одну строку
                new InlineKeyboardButton()
                {
                    Text = "📌Без списка",
                    CallbackData = "show"
                } 
            });
            IReadOnlyList<ToDoList> userLists = await _toDoListService.GetUserLists((await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct)).UserId, ct);
            foreach(ToDoList list in userLists)
            {
                buttons.Add(new[] { new InlineKeyboardButton() { Text = list.Name, CallbackData = ToDoListCallbackDto.FromString($"show|{list.Id}").ToString() } });
            }
            buttons.Add(new[]
            {
                new InlineKeyboardButton()
                {Text = "🆕Добавить", CallbackData = "addlist"},
                new InlineKeyboardButton()
                {Text = "❌Удалить", CallbackData = "deletelist"}
            });
            await botClient.SendMessage(update.Message.Chat, "Выберите список", replyMarkup: new InlineKeyboardMarkup(buttons), cancellationToken: ct);
            //Guid guid = (await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct)).UserId;
            //IReadOnlyList<ToDoItem> data = new List<ToDoItem>();
            //string result = "\r\n";
            //if(isActive)
            //    data = await _toDoService.GetActiveByUserIdAsync(guid, ct);
            //else
            //    data = await _toDoService.GetAllByUserIdAsync(guid, ct);
            //int i = 1;
            //foreach(ToDoItem Task in data)
            //{
            //    if(isActive)
            //        result += $"{i++})ID:`{Task.id}`, Название:{Task.Name}, Дата создания:{Task.CreatedAt}, Дедлайн:{Task.DeadLine}\r\n";
            //    else
            //        result += $"{i++})ID:`{Task.id}`, Название:{Task.Name}, Дата создания:{Task.CreatedAt}, Дедлайн:{Task.DeadLine}, Статус:{Task.State}, Изменение статуса:{Task.StateChangedAt}\r\n";
            //}
            //result = result.Remove(result.Length - 2);
            //result = EscapeString(result);
            //if (result == string.Empty)
            //    await botClient.SendMessage(update.Message.Chat, "Задач в списке нет", cancellationToken: ct, parseMode: ParseMode.MarkdownV2);
            //else
            //    await botClient.SendMessage(update.Message.Chat, result, cancellationToken: ct, parseMode: ParseMode.MarkdownV2);
        }
        private string EscapeString(string str)
        {
            char[] esc = new char[] { '\\', '*', '_', '{', '}', '[', ']', '(', ')', '#', '+', /*'-',*/ '.', '!' };
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
            var tasks = await _toDoService.FindAsync(guid, namePrefix, ct);
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
            ToDoUser? User = await _userService.GetUserByTelegramUserIdAsync(update.Message.From.Id, ct);
            ReplyKeyboardMarkup keyboard = new ReplyKeyboardMarkup(new List<KeyboardButton>
                {
                    new KeyboardButton("/addtask"),
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
        private async Task<bool> IsRegistered(ITelegramBotClient bot,Message message,CancellationToken ct)
        {
            if (await _userService.GetUserByTelegramUserIdAsync(message.From.Id,ct) == null)
            {
                await bot.SendMessage(message.Chat, "Команда доступна только для зарегистрированных пользователей. /start Для запуска.", cancellationToken: ct);
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
                "/addtask - добавить задачу\r\n" +
                "/show - показать список задач\r\n" +
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