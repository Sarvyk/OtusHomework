using LinqToDB;
using LinqToDB.Mapping;

namespace ConsoleApp1.Infrastructure.Models
{
    [Table("ToDoList")]
    internal class ToDoListModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }

        [Column("external_id")]
        public Guid ExternalId { get; set; }
        [Column("ListName"), NotNull]
        public string ListName { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UserId"), NotNull]
        public int UserId { get; set; }

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoUserModel.Id))]
        public ToDoUserModel User { get; set; }

        [Association(ThisKey = nameof(Id), OtherKey = nameof(ToDoItemModel.ToDoListId))]
        public List<ToDoItemModel> ToDoItems { get; set; }
    }
}
