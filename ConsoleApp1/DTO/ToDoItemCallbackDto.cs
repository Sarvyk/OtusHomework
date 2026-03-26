using ConsoleApp1.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.DTO
{
    internal class ToDoItemCallbackDto : CallbackDto
    {
        public Guid ToDoItemId;
        public static new ToDoItemCallbackDto FromString(string input)
        {
            string[] values = input.Split('|');
            ToDoItemCallbackDto dto = new ToDoItemCallbackDto();
            dto.Action = values[0];
            dto.ToDoItemId = Guid.Parse(values[1]);
            return dto;
        }
        public override string ToString() => $"{base.ToString()}|{ToDoItemId}";
    }
}