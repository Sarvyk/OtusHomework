using ConsoleApp1.Core.Scenarios.Enums;

namespace ConsoleApp1.Core.Scenarios
{
    internal class ScenarioContext
    {
        public ScenarioType CurrentScenario { get; set; }//Тут тип сценария. В данный момент видимо только добавление задачи "AddTask"
        public string? CurrentStep { get; set; }//Текущий шаг
        public Dictionary<string, object> Data {  get; set; }
        public DateTime CreatedAt { get; }
        public ScenarioContext(ScenarioType scenario)
        {
            CurrentScenario = scenario;
            CurrentStep = null;
            Data = new Dictionary<string, object>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}