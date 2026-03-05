namespace ConsoleApp1.Core.Exceptions
{
    internal class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit) : base($"Превышено максимальное количество задач. Максимальное количество '{taskCountLimit}'")
        {
        }
    }
}
