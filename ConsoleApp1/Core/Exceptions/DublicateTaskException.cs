namespace ConsoleApp1.Core.Exceptions
{
    internal class DublicateTaskException : Exception
    {
        public DublicateTaskException(string task) : base($"Задача '{task}' уже существует!")
        {
        }
    }
}
