using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Entities
{
    internal class ToDoItem
    {
        public Guid id { get; }
        public ToDoUser User { get;}
        public string Name { get; }
        public DateTime CreatedAt { get; }
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
        public DateTime? StateChangedAt { get; private set; }
        public ToDoItem(ToDoUser user, string name)
        {
            id = Guid.NewGuid();
            User = user;
            Name = name;
            CreatedAt = DateTime.UtcNow;
            State = ToDoItemState.Active;
        }
    }
}
