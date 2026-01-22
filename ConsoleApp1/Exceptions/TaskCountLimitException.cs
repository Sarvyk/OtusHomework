namespace ConsoleApp1.Exceptions
{
    internal class TaskCountLimitException : Exception
    {
        public TaskCountLimitException(int taskCountLimit) : base($"Превышено максимальное количество задач. Максимальное количество '{taskCountLimit}'")
        {
        }
    }
}
