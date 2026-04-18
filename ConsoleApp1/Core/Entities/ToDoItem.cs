using ConsoleApp1.Core.Entities.Enums;
using Telegram.Bot.Types;

namespace ConsoleApp1.Core.Entities
{
    internal class ToDoItem
    {
        public Guid Id { get; set; }
        public int? DatabaseId { get; set; }
        public Guid UserId { get; set; }
        public int? UserDatabaseId { get; set; }
        public ToDoUser User { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public ToDoItemState State { get; set; }
        public DateTime DeadLine { get; set; }
        public DateTime? StateChangedAt { get; set; }
        public int? ToDoListDatabaseId { get; set; }
        public ToDoList? ToDoList { get; set; }
    }
}