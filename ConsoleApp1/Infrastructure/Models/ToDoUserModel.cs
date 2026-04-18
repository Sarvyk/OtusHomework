using LinqToDB;
using LinqToDB.Mapping;

namespace ConsoleApp1.Infrastructure.Models
{
    [Table("ToDoUser")]
    internal class ToDoUserModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }
        [Column("external_id")]
        public Guid ExternalId { get; set; }
        [Column("Telegram_UserId"), NotNull]
        public long TelegramUserId { get; set; }

        [Column("Telegram_UserName"), NotNull]
        public string TelegramUserName { get; set; }

        [Column("Registered_At")]
        public DateTime RegisteredAt { get; set; }

        [Association(ThisKey = nameof(Id), OtherKey = nameof(ToDoListModel.UserId))]
        public List<ToDoListModel> ToDoLists { get; set; }

        [Association(ThisKey = nameof(Id), OtherKey = nameof(ToDoItemModel.UserId))]
        public List<ToDoItemModel> ToDoItems { get; set; }
    }
}
