using Otus.ToDoList.ConsoleBot;
using Otus.ToDoList.ConsoleBot.Types;
using System.Reflection;


namespace ConsoleApp1.Classes
{
    internal class UpdateHandler : IUpdateHandler
    {
        private readonly IUserService UserService;
        private readonly IToDoService ToDoService;
        public UpdateHandler(IUserService userService, IToDoService toDoService)
        {
            UserService = userService;
            ToDoService = toDoService;
        }
        public void HandleUpdateAsync(ITelegramBotClient botClient, Update update)
        {
            try
            {
                switch(update.Message.Text)
                {
                    case "/start":
                        botClient.SendMessage(update.Message.Chat, StartCommand(botClient,update));
                        botClient.SendMessage(update.Message.Chat, $"{HelpCommand(update)}");
                        break;
                    case "/help":
                        botClient.SendMessage(update.Message.Chat, $"{HelpCommand(update)}");
                        break;
                    case "/info":
                        botClient.SendMessage(update.Message.Chat, $"{InfoCommand()}");
                        break;
                    case string a when a.IndexOf("/addtask") == 0:
                        if (IsRegistered(botClient,update))
                        {
                            ToDoService.Add(UserService.GetUser(update.Message.From.Id), a.Replace("/addtask", "").Trim());
                            botClient.SendMessage(update.Message.Chat, "Задача успешно добавлена");
                        }
                        break;
                    case string a when a.IndexOf("/completetask") == 0:
                        if (IsRegistered(botClient,update))
                        {
                            Guid guid = new Guid();
                            if (!Guid.TryParse(a.Replace("/completetask", ""), out guid))
                                throw new ArgumentException("Такого id нет!");
                            ToDoService.MarkCompleted(guid);
                            botClient.SendMessage(update.Message.Chat, "Задача завершена");
                        }
                        break;
                    case string a when a.IndexOf("/removetask") == 0:
                        if (IsRegistered(botClient,update))
                        {
                            Guid guid = new Guid();
                            if (!Guid.TryParse(a.Replace("/removetask", ""), out guid))
                                throw new ArgumentException("Такого id нет!");
                            ToDoService.Delete(guid);
                            botClient.SendMessage(update.Message.Chat, "Задача успешно удалена");
                        }
                        break;
                    case "/showtask":
                        if(IsRegistered(botClient,update))
                        {
                            ShowTasks(botClient, update);
                        }
                        break;
                    case "/showalltask":
                        if (IsRegistered(botClient,update))
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
        }
        private void ShowTasks(ITelegramBotClient bot, Update update, bool isActive = false)
        {
            Guid guid = UserService.GetUser(update.Message.From.Id).UserId;
            if (!isActive)
            {//Вывести все задачи.
                if (ToDoService.GetActiveByUserId(guid).Count == 0)
                {
                    bot.SendMessage(update.Message.Chat, "Задач в списке нет.");
                    return;
                }
                int i = 1;
                foreach (ToDoItem Task in ToDoService.GetActiveByUserId(guid))
                {
                    bot.SendMessage(update.Message.Chat, $"{i++})ID:{Task.id}, Название:{Task.Name}, Дата создания:{Task.CreatedAt}");
                }
            }
            else
            {//Вывести только те, что активны.
                if (ToDoService.GetAllByUserId(guid).Count == 0)
                {
                    bot.SendMessage(update.Message.Chat, "Задач в списке нет.");
                    return;
                }
                int i = 1;
                foreach (ToDoItem Task in ToDoService.GetAllByUserId(guid))
                {
                    bot.SendMessage(update.Message.Chat, $"{i++})ID:{Task.id}, Название:{Task.Name}, Дата создания:{Task.CreatedAt}, Статус:{Task.State}, Изменение статуса:{Task.StateChangedAt}");
                }
            }
        }
        private string StartCommand(ITelegramBotClient bot, Update update)
        {
            ToDoUser? User = UserService.GetUser(update.Message.From.Id);
            if (User != null)
            {
                return $"{User.TelegramUserName}, команда уже выполнена.";
            }
            else
            {
                User = UserService.RegisterUser(update.Message.From.Id, update.Message.From.Username);
                return $"{User.TelegramUserName}, добро пожаловать!";
            }
        }
        private bool IsRegistered(ITelegramBotClient bot,Update update)
        {
            if (UserService.GetUser(update.Message.From.Id) == null)
            {
                bot.SendMessage(update.Message.Chat, "Команда доступна только для зарегистрированных пользователей. /start Для запуска.");
                return false;
            }
            else
                return true;
        }
        private string HelpCommand(Update update)
        {
            if (UserService.GetUser(update.Message.From.Id) != null)
                return $"Используйте следующий список команд для работы:\r\n" +
                "/help - вывод помощи\r\n" +
                "/info - вывод информации по программе\r\n" +
                "/addtask - добавить задачу\r\n" +
                "/showtask - показать список задач\r\n" +
                "/completetask - завершить задачу\r\n" +
                "/showalltask - показать все задачи\r\n" +
                "/removetask - удалить задачу из списка";
            else
                return "Для не зарегестрированного пользователя доступны только команды /start, /help и /info";
        }
        private string InfoCommand()
        {
            return $"Текущая версия программы {Assembly.GetEntryAssembly().GetName().Version.ToString()}. Дата создания {DateTime.Now.ToString("d")}";
        }
    }
}
