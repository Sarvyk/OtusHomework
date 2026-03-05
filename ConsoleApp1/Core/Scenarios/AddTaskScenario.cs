using ConsoleApp1.Core.Interfaces.DataAccess;
using ConsoleApp1.Core.Scenarios.Enums;
using ConsoleApp1.Core.Scenarios.Interfaces;
using ConsoleApp1.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp1.Core.Scenarios
{
    internal class AddTaskScenario : IScenario
    {
        private readonly IUserService _userService;
        private readonly IToDoService _toDoService;
        public AddTaskScenario(IUserService userService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenario)
        {
            return scenario == ScenarioType.AddTask;
        }

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Message message, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    context.CurrentStep = "Name";
                    await botClient.SendMessage(message.Chat, "Введите название задачи", replyMarkup: ReplyKeyboardManager.SetCancelButton(), cancellationToken: ct);
                    return ScenarioResult.Transition;
                case "Name":
                    context.Data.Add("Name", message.Text);
                    context.CurrentStep = "DeadLine";
                    await botClient.SendMessage(message.Chat, $"Введите дату дедлайна Формат должен быть \"{DateTime.Now.ToShortDateString()}\"", replyMarkup: ReplyKeyboardManager.SetCancelButton(), cancellationToken: ct);
                    return ScenarioResult.Transition;
                case "DeadLine":
                    if (!DateTime.TryParse(message.Text, new CultureInfo("ru-RU"),  out DateTime resultDL))
                    {
                        await botClient.SendMessage(message.Chat, $"Не верно введённая дата. Формат должен быть \"{DateTime.Now.ToShortDateString()}\" Попробуйте снова", replyMarkup: ReplyKeyboardManager.SetCancelButton(), cancellationToken: ct);
                        return ScenarioResult.Transition;
                    }
                    if (resultDL < DateTime.Now)
                    {
                        await botClient.SendMessage(message.Chat, "Дата дедлайна не может быть меньше или равна текущей даты. Попробуйте снова", replyMarkup: ReplyKeyboardManager.SetCancelButton(), cancellationToken: ct);
                        return ScenarioResult.Transition;
                    }
                    context.Data.Add("DeadLine", message.Text);
                    break;
            }
            await _toDoService.AddAsync(await _userService.GetUserByTelegramUserIdAsync(message.From.Id, ct), context.Data["Name"].ToString(), Convert.ToDateTime(context.Data["DeadLine"].ToString()), ct);
            await botClient.SendMessage(message.Chat, "Задача успешно добавлена", replyMarkup: ReplyKeyboardManager.SetStandartListButton(), cancellationToken: ct);
            return ScenarioResult.Completed;
        }
    }
}