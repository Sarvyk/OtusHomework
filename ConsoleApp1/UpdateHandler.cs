using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Entities.Enums;
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
        private static int _pageSize = 5;
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
                    if (a.Data.StartsWith("showtask"))
                    {
                        ToDoItemCallbackDto itemDTO = ToDoItemCallbackDto.FromString(a.Data);
                        ToDoItem task = await _toDoService.Get(itemDTO.ToDoItemId, ct);
                        string answer = string.Empty;
                        InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup();
                        if (task.State == ToDoItemState.Active)
                        {
                            answer = $"{task.Name}\r\nСрок выполнения:{task.DeadLine}\r\nВремя выполнения:{task.CreatedAt}";
                            keyboardMarkup.AddNewRow(new InlineKeyboardButton[]
                            {
                            new InlineKeyboardButton("✅Выполнить",ToDoItemCallbackDto.FromString($"completetask|{itemDTO.ToDoItemId}").ToString()),
                            new InlineKeyboardButton("❌Удалить",ToDoItemCallbackDto.FromString($"deletetask|{itemDTO.ToDoItemId}").ToString())
                            });
                        }
                        else
                            answer = $"{task.Name}\r\nСрок выполнения:{task.DeadLine}\r\nВремя выполнения:{task.CreatedAt}\r\nВремя завершения:{task.StateChangedAt}";
                        await botClient.SendMessage(update.CallbackQuery.Message.Chat, answer, replyMarkup: keyboardMarkup, cancellationToken: ct);
                        break;
                    }
                    PagedListCallbackDto listDTO = PagedListCallbackDto.FromString(a.Data);
                    IReadOnlyList<ToDoItem> tasks = null;
                    Guid userId = (await _userService.GetUserByTelegramUserIdAsync(update.CallbackQuery.Message.From.Id, ct)).UserId;
                    if (listDTO.Action == "show" && listDTO.ToDoListId != null)//получаем список активных задач с привязкой к списку.
                        tasks = (await _toDoService.GetByUserIdAndList(userId, listDTO.ToDoListId, ct)).Where(task => task.State == ToDoItemState.Active).ToList();
                    else if(listDTO.Action == "show")//список активных задач без привязки к списку.
                        tasks = await _toDoService.GetActiveByUserIdAsync(userId, ct);
                    else if (listDTO.Action == $"show_completed" && listDTO.ToDoListId != null)//список завершённых задач с привязкой к списку.
                        tasks = (await _toDoService.GetByUserIdAndList(userId, listDTO.ToDoListId, ct)).Where(task => task.State == ToDoItemState.Completed).ToList();
                    else if (listDTO.Action == $"show_completed")//список завершённых задач без привязки к списку.
                        tasks = await _toDoService.GetCompletedByUserIdAsync(userId, ct);
                    List<KeyValuePair<string, string>> callbackData = new List<KeyValuePair<string, string>>();
                    foreach (ToDoItem task in tasks)
                    {
                        callbackData.Add(new KeyValuePair<string, string>(task.Name, ToDoItemCallbackDto.FromString($"showtask|{task.id}").ToString()));
                    }

                    if (tasks.Count == 0)
                        await botClient.SendMessage(update.CallbackQuery.Message.Chat, (listDTO.ToDoListId != null ? "Задач в списке нет" : "Задачи отсутствуют"), cancellationToken: ct);
                    else
                        await botClient.EditMessageText(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, "Активные задачи", replyMarkup: BuildPagedButtons(callbackData, listDTO), cancellationToken: ct);
                    break;
                case CallbackQuery a when a.Data.StartsWith("completetask"):
                    ToDoItemCallbackDto tdo = ToDoItemCallbackDto.FromString(a.Data);
                    await _toDoService.MarkCompletedAsync(tdo.ToDoItemId, ct);
                    await botClient.EditMessageReplyMarkup(update.CallbackQuery.Message.Chat.Id, update.CallbackQuery.Message.MessageId, null, cancellationToken: ct);
                    await botClient.SendMessage(update.CallbackQuery.Message.Chat, "Задача завершена", replyMarkup:MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                    break;
                case CallbackQuery a when a.Data.StartsWith("deletetask"):
                    context = new ScenarioContext(ScenarioType.DeleteTask);
                    context.Data.Add("Callback", ToDoItemCallbackDto.FromString(a.Data).ToString());
                    await _scenarioContextRepository.SetContext(update.CallbackQuery.From.Id, context, ct);
                    await ProcessScenario(botClient, context, update.CallbackQuery.From, update.CallbackQuery.Message, ct);
                    ToDoItemCallbackDto tdo2 = ToDoItemCallbackDto.FromString(a.Data);
                    break;
            }
        }
        private InlineKeyboardMarkup BuildPagedButtons(IReadOnlyList<KeyValuePair<string, string>> callbackData, PagedListCallbackDto listDto)
        {
            InlineKeyboardMarkup keyboardMarkup = new InlineKeyboardMarkup();
            int allCount = callbackData.Count;
            int totalPage = (int)Math.Round((decimal)callbackData.Count / _pageSize,MidpointRounding.ToPositiveInfinity);//расчёт количества страниц.
            callbackData = callbackData.GetBatchByNumber(_pageSize, listDto.Page).ToList();//берём только те элементы, где страница равна той, которая указана во втором параметре.
            foreach (KeyValuePair<string, string> keyVal in callbackData)
            {
                keyboardMarkup.AddNewRow(new InlineKeyboardButton(keyVal.Key, keyVal.Value));
            }
            if (allCount > _pageSize)
            {
                if (listDto.Page == 0)
                {//настраиваем кнопки перехода по страницам
                    keyboardMarkup.AddNewRow(new InlineKeyboardButton("➡️", PagedListCallbackDto.FromString($"show|{listDto.ToDoListId}|{listDto.Page + 1}").ToString()));
                }
                else if (listDto.Page > 0 && listDto.Page < totalPage - 1)
                {
                    keyboardMarkup.AddNewRow(new InlineKeyboardButton[]
                    {
                    new InlineKeyboardButton("⬅️",PagedListCallbackDto.FromString($"show|{listDto.ToDoListId}|{listDto.Page - 1}").ToString()),
                    new InlineKeyboardButton("➡️",PagedListCallbackDto.FromString($"show|{listDto.ToDoListId}|{listDto.Page + 1}").ToString())
                    });
                }
                else
                {
                    keyboardMarkup.AddNewRow(new InlineKeyboardButton("⬅️", PagedListCallbackDto.FromString($"show|{listDto.ToDoListId}|{listDto.Page - 1}").ToString()));
                }
            }
            if(listDto.Action != "show_completed")
                keyboardMarkup.AddNewRow(new InlineKeyboardButton("Посмотреть выполненные", PagedListCallbackDto.FromString($"show_completed|{listDto.ToDoListId}|0").ToString()));
            return keyboardMarkup;
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
            foreach (ToDoList list in userLists)
            {
                buttons.Add(new[] { new InlineKeyboardButton() { Text = list.Name, CallbackData = PagedListCallbackDto.FromString($"show|{list.Id}|0").ToString() } });
            }
            buttons.Add(new[]
            {
                new InlineKeyboardButton()
                {Text = "🆕Добавить", CallbackData = "addlist"},
                new InlineKeyboardButton()
                {Text = "❌Удалить", CallbackData = "deletelist"}
            });
            await botClient.SendMessage(update.Message.Chat, "Выберите список", replyMarkup: new InlineKeyboardMarkup(buttons), cancellationToken: ct);
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