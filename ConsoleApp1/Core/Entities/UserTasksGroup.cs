using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Core.Entities
{
    internal class UserTasksGroup
    {
        public int UserId { get; set; }
        public List<ToDoItem> Tasks { get; set; }
    }
}