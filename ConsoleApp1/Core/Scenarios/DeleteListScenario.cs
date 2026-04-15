using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Interfaces.DataAccess;
using ConsoleApp1.Core.Scenarios.Enums;
using ConsoleApp1.Core.Scenarios.Interfaces;
using ConsoleApp1.DTO;
using ConsoleApp1.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Core.Scenarios
{
    internal class DeleteListScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;
        private readonly IToDoService _toDoService;
        public DeleteListScenario(IUserService userService, IToDoListService toDoListService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenario) => scenario == ScenarioType.DeleteList;

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Message message, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    //ToDoUser user = await _userService.GetUserByTelegramUserIdAsync(message.From.Id, ct);
                    //context.Data.Add("User", user);
                    IReadOnlyList<ToDoList> userLists = await _toDoListService.GetUserLists((await _userService.GetUserByTelegramUserIdAsync(long.Parse(context.Data["TelegramUserId"].ToString()), ct)).UserId, ct);
                    if (userLists.Count == 0)
                    {
                        throw new ArgumentException("Не обнаружены списки");
                    }
                    context.Data.Add("Lists",userLists);
                    List<InlineKeyboardButton[]> listButtons = new List<InlineKeyboardButton[]>();
                    foreach(ToDoList list in userLists)
                    {
                        listButtons.Add(new[] { new InlineKeyboardButton() { Text = list.Name, CallbackData = ToDoListCallbackDto.FromString($"deletelist|{list.Id}").ToString() } });
                    }
                    context.Data.Add("Callback","");//Для переноса ответа.
                    await botClient.SendMessage(message.Chat, "Выберете список для удаления:", replyMarkup: new InlineKeyboardMarkup(listButtons), cancellationToken: ct);
                    context.CurrentStep = "Approve";
                    return ScenarioResult.Transition;
                case "Approve":
                    ToDoList selectedList = null;
                    foreach(ToDoList list in (IReadOnlyList<ToDoList>)(context.Data["Lists"]))
                    {
                        if(list.Id.ToString() == (context.Data["Callback"]).ToString().Split('|')[1])
                        {
                            selectedList = list;
                            context.Data["SelectedList"] = selectedList;
                            break;
                        }
                    }
                    await botClient.SendMessage(message.Chat, $"Подтверждаете удаление списка \"{selectedList.Name}\"", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton("✅Да", "yes"), new InlineKeyboardButton("❌Нет", "no")), cancellationToken: ct);
                    context.CurrentStep = "Delete";
                    return ScenarioResult.Transition;
                case "Delete":
                    if (context.Data["Callback"].ToString() == "no")
                    {
                        await botClient.SendMessage(message.Chat, "Удаление отменено.", replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                        return ScenarioResult.Completed;
                    }
                    Guid userId = (await _userService.GetUserByTelegramUserIdAsync(long.Parse(context.Data["TelegramUserId"].ToString()), ct)).UserId;
                    Guid listId = ((ToDoList)(context.Data["SelectedList"])).Id;
                    foreach(ToDoItem toDoItem in await _toDoService.GetByUserIdAndList(userId, listId, ct))
                    {
                        await _toDoService.DeleteAsync(toDoItem.Id,ct);
                        await FileLinkIndex.RemoveTaskIndex(toDoItem.Id.ToString());
                    }
                    await _toDoListService.Delete(listId, ct);
                    await FileLinkIndex.RemoveTaskListIndex(listId.ToString());
                    break;
            }
            await botClient.SendMessage(message.Chat, "Лист удалён.", replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
            return ScenarioResult.Completed;
        }
    }
}
