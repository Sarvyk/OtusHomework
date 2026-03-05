using ConsoleApp1.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Scenarios
{
    internal class InMemoryScenarioContextRepository : IScenarioContextRepository
    {
        private readonly Dictionary<long, ScenarioContext> _context = new();//тут будут храниться сценарии пользователей. Типа диалоги с сохранением состояния
        public async Task<ScenarioContext?> GetContext(long userId, CancellationToken ct)
        {
            if (_context.ContainsKey(userId))
                return _context[userId];
            return null;
        }

        public async Task ResetContext(long userId, CancellationToken ct)
        {
            if (_context.ContainsKey(userId))
                _context.Remove(userId);
        }

        public async Task SetContext(long userId, ScenarioContext context, CancellationToken ct)
        {
            if (_context.ContainsKey(userId))
                _context[userId] = context;
            else
                _context.Add(userId, context);
        }
    }
}