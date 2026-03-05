using ConsoleApp1.Core.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Core.Entities
{
    internal class ToDoItem
    {
        public Guid id { get; set; }
        public ToDoUser User { get; set; }
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        private ToDoItemState _state;
        public ToDoItemState State { 
            get 
            { 
                return _state;
            }
            set 
            {
                _state = value;
                StateChangedAt = DateTime.UtcNow;
            }
        }
        public DateTime DeadLine { get; set; }
        public DateTime? StateChangedAt { get; private set; }
        public ToDoItem() {}
        public ToDoItem(ToDoUser user, string name, DateTime deadLine)
        {
            id = Guid.NewGuid();
            User = user;
            Name = name;
            CreatedAt = DateTime.UtcNow;
            State = ToDoItemState.Active;
            DeadLine = deadLine;
        }
    }
}
