using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Interfaces.DataAccess;
using ConsoleApp1.Core.Scenarios.Enums;
using ConsoleApp1.Core.Scenarios.Interfaces;
using ConsoleApp1.Core.Services;
using ConsoleApp1.DTO;
using ConsoleApp1.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Core.Scenarios
{
    internal class DeleteTaskScenario : IScenario
    {
        private readonly IToDoService _toDoService;
        public DeleteTaskScenario(IToDoService toDoService)
        {
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenario) => scenario == ScenarioType.DeleteTask;

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Message message, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    Guid taskId = ToDoItemCallbackDto.FromString(context.Data["Callback"].ToString()).ToDoItemId;
                    context.Data.Add("taskId", taskId.ToString());
                    string taskName = (await _toDoService.Get(taskId, ct)).Name;
                    await botClient.SendMessage(message.Chat, $"Подтверждаете удаление задачи \"{taskName}\"", replyMarkup: new InlineKeyboardMarkup(new InlineKeyboardButton("✅Да", "yes"), new InlineKeyboardButton("❌Нет", "no")), cancellationToken: ct);
                    context.CurrentStep = "Delete";
                    return ScenarioResult.Transition;
                case "Delete":
                    if (context.Data["Callback"].ToString() == "no")
                    {
                        await botClient.SendMessage(message.Chat, "Удаление отменено.", replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                        return ScenarioResult.Completed;
                    }
                    await _toDoService.DeleteAsync(Guid.Parse((context.Data["taskId"].ToString())),ct);
                    await botClient.SendMessage(message.Chat, "Задача удалена.", replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                    break;
            }
            return ScenarioResult.Completed;
        }
    }
}