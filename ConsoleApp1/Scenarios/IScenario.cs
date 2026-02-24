using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp1.Scenarios
{
    internal interface IScenario
    {
        bool CanHandle(ScenarioType scenario);
        Task<ScenarioResult> HandleMessageAsync(ITelegramBotClient bot, ScenarioContext context, Message message, CancellationToken ct);
    }
}