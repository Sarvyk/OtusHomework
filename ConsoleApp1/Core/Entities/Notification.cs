using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Core.Entities
{
    internal class Notification
    {
        public Guid Id { get; set; }
        public ToDoUser User { get; set; }
        public int? DatabaseId { get; set; }
        public string Type {get; set;}//Тип нотификации. Например: DeadLine_{ToDoItem.Id}, Today_{DateOnly.FromDateTime(DateTime.UtcNow)}
        public string Text {get; set;} //Текст, который будет отправлен
        public DateTime ScheduledAt {get; set;} //Запланированная дата отправки
        public bool IsNotified {get; set;} //Флаг отправки
        public DateTime? NotifiedAt {get; set;} //Фактическая дата отправки
    }
}
