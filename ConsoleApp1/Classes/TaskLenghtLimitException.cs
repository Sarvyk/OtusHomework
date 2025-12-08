namespace ConsoleApp1.Classes
{
    internal class TaskLenghtLimitException : Exception
    {
        public TaskLenghtLimitException(int taskLength, int taskLengthLimit) : base($"Длина задачи '{taskLength}' превышает максимально допустимое значение '{taskLengthLimit}'")
        {
        }
    }
}
