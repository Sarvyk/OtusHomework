namespace ConsoleApp1.Classes
{
    internal class DublicateTaskException : Exception
    {
        public DublicateTaskException(string task) : base($"Задача '{task}' уже существует!")
        {
        }
    }
}
