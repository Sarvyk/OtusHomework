using ConsoleApp1.Core.Scenarios;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Core.Scenarios.Interfaces
{
    internal interface IScenarioContextRepository
    {
        Task<ScenarioContext?> GetContext(long userId, CancellationToken ct);
        Task SetContext(long userId, ScenarioContext context, CancellationToken ct);
        Task ResetContext(long userId, CancellationToken ct);
    }
}