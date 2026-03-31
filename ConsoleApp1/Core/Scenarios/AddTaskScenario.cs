using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Interfaces.DataAccess;
using ConsoleApp1.Core.Scenarios.Enums;
using ConsoleApp1.Core.Scenarios.Interfaces;
using ConsoleApp1.DTO;
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
        private readonly IToDoListService _toDoListService;
        public AddTaskScenario(IUserService userService, IToDoListService toDoListService, IToDoService toDoService)
        {
            _userService = userService;
            _toDoListService = toDoListService;
            _toDoService = toDoService;
        }
        public bool CanHandle(ScenarioType scenario) => scenario == ScenarioType.AddTask;

        public async Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient botClient, ScenarioContext context, Message message, CancellationToken ct)
        {
            switch (context.CurrentStep)
            {
                case null:
                    context.CurrentStep = "Name";
                    await botClient.SendMessage(message.Chat, "Введите название задачи", replyMarkup: MarkupManager.SetKeyboardCancel(), cancellationToken: ct);
                    return ScenarioResult.Transition;
                case "Name":
                    context.Data.Add("Name", message.Text);
                    context.CurrentStep = "DeadLine";
                    await botClient.SendMessage(message.Chat, $"Введите дату дедлайна Формат должен быть \"{DateTime.Now.ToShortDateString()}\"", replyMarkup: MarkupManager.SetKeyboardCancel(), cancellationToken: ct);
                    return ScenarioResult.Transition;
                case "DeadLine":
                    if (!DateTime.TryParse(message.Text, new CultureInfo("ru-RU"),  out DateTime resultDL))
                    {
                        await botClient.SendMessage(message.Chat, $"Не верно введённая дата. Формат должен быть \"{DateTime.Now.ToShortDateString()}\" Попробуйте снова", replyMarkup: MarkupManager.SetKeyboardCancel(), cancellationToken: ct);
                        return ScenarioResult.Transition;
                    }
                    if (resultDL < DateTime.Now)
                    {
                        await botClient.SendMessage(message.Chat, "Дата дедлайна не может быть меньше или равна текущей даты. Попробуйте снова", replyMarkup: MarkupManager.SetKeyboardCancel(), cancellationToken: ct);
                        return ScenarioResult.Transition;
                    }
                    context.Data.Add("DeadLine", message.Text);
                    List<InlineKeyboardButton[]> listButtons = new List<InlineKeyboardButton[]>();
                    IReadOnlyList<ToDoList> lists = await _toDoListService.GetUserLists((await _userService.GetUserByTelegramUserIdAsync(message.From.Id, ct)).UserId, ct);
                    context.Data["Lists"] = lists;
                    foreach(ToDoList list in lists)
                    {
                        listButtons.Add(new[] { new InlineKeyboardButton() { Text = list.Name, CallbackData = ToDoListCallbackDto.FromString($"selectList|{list.Id}").ToString() } });
                    }
                    await botClient.SendMessage(message.Chat, "Выберите список, в которой нужно добавить задачу:", replyMarkup: new InlineKeyboardMarkup(listButtons), cancellationToken: ct);
                    context.CurrentStep = "SelectList";
                    return ScenarioResult.Transition;
                case "SelectList":
                    ToDoList selectedList = null;
                    foreach(ToDoList list in (IReadOnlyList<ToDoList>)context.Data["Lists"])
                    {
                        if(list.Id.ToString() == context.Data["Callback"].ToString().Split("|")[1])
                        {
                            selectedList = list;
                            break;
                        }
                    }
                    await _toDoService.AddAsync(await _userService.GetUserByTelegramUserIdAsync(message.From.Id, ct), context.Data["Name"].ToString(), Convert.ToDateTime(context.Data["DeadLine"].ToString()), selectedList, ct);
                    break;
            }
            await botClient.SendMessage(message.Chat, "Задача успешно добавлена", replyMarkup: MarkupManager.SetStandartKeyboardButtonList(), cancellationToken: ct);
            return ScenarioResult.Completed;
        }
    }
}