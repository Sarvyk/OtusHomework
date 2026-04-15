using LinqToDB;
using LinqToDB.Mapping;

namespace ConsoleApp1.Infrastructure.Models
{
    [Table("ToDoItem")]
    internal class ToDoItemModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }

        [Column("external_id")]
        public Guid ExternalId { get; set; }

        [Column("ItemName"), NotNull]
        public string ItemName { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("ItemState"), NotNull]
        public int ItemState { get; set; }

        [Column("DeadLine")]
        public DateTime DeadLine { get; set; }

        [Column("StateChangedAt")]
        public DateTime StateChangedAt { get; set; }

        [Column("UserId"), NotNull]
        public int UserId { get; set; }

        [Column("ToDoListId")]
        public int? ToDoListId { get; set; }

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoUserModel.Id))]
        public ToDoUserModel User { get; set; }

        [Association(ThisKey = nameof(ToDoListId), OtherKey = nameof(ToDoListModel.Id), CanBeNull = true)]
        public ToDoListModel List { get; set; }
    }
}
