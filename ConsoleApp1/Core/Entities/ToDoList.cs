
namespace ConsoleApp1.Core.Entities
{
    internal class ToDoList
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ToDoUser User { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoList() { }
        public ToDoList(ToDoUser user, string name)
        {
            Id = Guid.NewGuid();
            Name = name;
            User = user;
            CreatedAt = DateTime.UtcNow;
        }
    }
}