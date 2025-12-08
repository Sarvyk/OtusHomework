namespace ConsoleApp1.Classes
{
    internal class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit) : base($"Превышено максимальное количество задач. Максимальное количество '{taskCountLimit}'")
        {
        }
    }
}
