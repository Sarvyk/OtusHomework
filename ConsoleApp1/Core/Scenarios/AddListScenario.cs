using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Interfaces.DataAccess;
using ConsoleApp1.Core.Scenarios.Enums;
using ConsoleApp1.Core.Scenarios.Interfaces;
using ConsoleApp1.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Core.Scenarios
{
    internal class AddListScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoListService _toDoListService;
        public AddListScenario(IUserService userService, IToDoListService toDoListService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddList;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Message message, CancellationToken ct)
        {
            switch(context.CurrentStep)
            {
                case null:
                    ToDoUser user = await _userService.GetUserByTelegramUserIdAsync(message.From.Id, ct);
                    context.Data.Add("User", user);
                    await botClient.SendMessage(message.Chat, "Введите название списка:", replyMarkup: MarkupManager.SetKeyboardCancel(), cancellationToken: ct);
                    context.CurrentStep = "Name";
                    return ScenarioResult.Transition;
                case "Name":
                    await _toDoListService.Add((ToDoUser)(context.Data["User"]), message.Text, ct);
                    await botClient.SendMessage(message.Chat, $"Лист \"{message.Text}\" успешно добавлен!", replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
                    break;
            }
            return ScenarioResult.Completed;
        }
    }
}