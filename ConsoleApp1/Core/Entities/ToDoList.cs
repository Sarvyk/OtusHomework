
namespace ConsoleApp1.Core.Entities
{
    internal class ToDoList
    {
        public Guid Id { get; set; }
        public int? DatabaseId { get; set; }
        public string Name { get; set; }
        public int? UserDatabaseId { get; set; }
        public ToDoUser User { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}