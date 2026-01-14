using ConsoleApp1.Classes;
using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

var handler = new UpdateHandler(new UserService(), new ToDoService());
var botClient = new ConsoleBotClient();
botClient.StartReceiving(handler);

namespace ConsoleApp1.Classes
{
    internal class UpdateHandler : IUpdateHandler
    {
        private ToDoUser User;
        IUserService UserService;
        ToDoService ToDoService;
        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            UserService = userService;
            ToDoService = (ToDoService)toDoService;
        }
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            try
            {
                switch(update.Message.Text)
                {
                    case "/start":
                        StartCommand(botClient,update);
                        botClient.SendMessage(update.Message.Chat, $"{HelpCommand()}");
                        break;
                    case "/help":
                        botClient.SendMessage(update.Message.Chat, $"{HelpCommand()}");
                        break;
                    case "/info":
                        botClient.SendMessage(update.Message.Chat, $"{InfoCommand()}");
                        break;
                    case string a when a.IndexOf("/addtask") == 0:
                        if (IsRegistered())
                        {
                            ToDoService.Add(User, a.Replace("/addtask", "").Trim());
                            botClient.SendMessage(update.Message.Chat, "Задача успешно добавлена");
                        }
                        break;
                    case string a when a.IndexOf("/completetask") == 0:
                        if (IsRegistered())
                        {
                            Guid G = ToDoService.GetTaskByNumber(a.Replace("/completetask", ""));
                            ToDoService.MarkCompleted(G);
                        }
                        break;
                    case string a when a.IndexOf("/removetask") == 0:
                        if (IsRegistered())
                        {
                            Guid G = ToDoService.GetTaskByNumber(a.Replace("/removetask",""));
                            ToDoService.Delete(G);
                            botClient.SendMessage(update.Message.Chat, "Задача успешно удалена");
                        }
                        break;
                    case "/showtask":
                        if(IsRegistered())
                        {
                            ShowTasks(botClient, update);
                        }
                        break;
                    case "/showalltask":
                        if (IsRegistered())
                        {
                            ShowTasks(botClient, update, true);
                        }
                            break;
                    default:
                        botClient.SendMessage(update.Message.Chat,"Такой команды не существует!");
                        break;
                }
            }
            catch (TaskCountLimitException ex)
            {
                botClient.SendMessage(update.Message.Chat, ex.Message);
            }
            catch (TaskLenghtLimitException ex)
            {
                botClient.SendMessage(update.Message.Chat, ex.Message);
            }
            catch (DublicateTaskException ex)
            {
                botClient.SendMessage(update.Message.Chat, ex.Message);
            }
            catch (ArgumentException ex)
            {
                botClient.SendMessage(update.Message.Chat, ex.Message);
            }
            if (!IsRegistered())
                botClient.SendMessage(update.Message.Chat, "Для не зарегестрированного пользователя доступны только команды /start, /help и /info");
        }
        private void ShowTasks(ITelegramBotClient bot, Update update, bool isActive = false)
        {
            if (!isActive)
            {//Вывести все задачи.
                if (ToDoService.GetActiveByUserId(User.UserId).Count == 0)
                {
                    bot.SendMessage(update.Message.Chat, "Задач в списке нет.");
                    return;
                }
                int i = 1;
                foreach (ToDoItem Task in ToDoService.GetActiveByUserId(User.UserId))
                {
                    bot.SendMessage(update.Message.Chat, $"{i++})ID:{Task.id}, Название:{Task.Name}, Дата создания:{Task.CreatedAt}");
                }
            }
            else
            {//Вывести только те, что активны.
                if (ToDoService.GetAllByUserId(User.UserId).Count == 0)
                {
                    bot.SendMessage(update.Message.Chat, "Задач в списке нет.");
                    return;
                }
                int i = 1;
                foreach (ToDoItem Task in ToDoService.GetAllByUserId(User.UserId))
                {
                    bot.SendMessage(update.Message.Chat, $"{i++})ID:{Task.id}, Название:{Task.Name}, Дата создания:{Task.CreatedAt}, Статус:{Task.State}, Изменение статуса:{Task.StateChangedAt}");
                }
            }
        }
        private string StartCommand(ITelegramBotClient bot, Update update)
        {
            if (User != null)
            {
                return $"{User.TelegramUserName}, команда уже выполнена.";
            }
            if (UserService.GetUser(6455631531) == null)
            {
                User = UserService.RegisterUser(update.Message.From.Id, update.Message.From.Username);
                ToDoService.SetMaxTask(bot, update);
                ToDoService.SetMaxTaskLength(bot, update);
                return $"{User.TelegramUserName}, добро пожаловать!";
            }
            return "";
        }
        private bool IsRegistered()
        {
            if (User == null)
            {
                return false;
            }
            else
                return true;
        }
        private string HelpCommand()
        {
            return $"Используйте следующий список команд для работы:\r\n" +
                "/help - вывод помощи\r\n" +
                "/info - вывод информации по программе\r\n" +
                "/addtask - добавить задачу\r\n" +
                "/showtask - показать список задач\r\n" +
                "/completetask - завершить задачу\r\n" +
                "/showalltask - показать все задачи\r\n" +
                "/removetask - удалить задачу из списка\r\n" +
                "/exit - выход из программы";
        }
        private string InfoCommand()
        {
            return $"Текущая версия программы {Assembly.GetEntryAssembly().GetName().Version.ToString()}. Дата создания {DateTime.Now.ToString("d")}";
        }
    }
}
