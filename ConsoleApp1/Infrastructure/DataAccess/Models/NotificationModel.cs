using ConsoleApp1.Core.Entities;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Infrastructure.DataAccess.Models
{
    [Table("Notification")]
    internal class NotificationModel
    {
        [PrimaryKey, Identity]
        [Column("id")]
        public int Id { get; set; }

        [Column("External_id")]
        public Guid ExternalId { get; set; }

        [Column("User_id"), NotNull]
        public int UserId { get; set; }

        [Column("Type"), NotNull]
        public string Type { get; set; }

        [Column("Text"), NotNull]
        public string Text { get; set; }

        [Column("Scheduled_at"), NotNull]
        public DateTime ScheduledAt { get; set; }

        [Column("Is_notified"), NotNull]
        public bool IsNotified { get; set; }

        [Column("Notified_at")]
        public DateTime? NotifiedAt { get; set; }

        [Association(ThisKey = nameof(UserId), OtherKey = nameof(ToDoUserModel.Id))]
        public ToDoUserModel User { get; set; }
    }
}
