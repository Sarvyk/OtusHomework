namespace ConsoleApp1.Exceptions
{
    internal class DublicateTaskException : Exception
    {
        public DublicateTaskException(string task) : base($"Задача '{task}' уже существует!")
        {
        }
    }
}
