using System.ComponentModel;

namespace ConsoleApp1.Core.Entities.Enums
{
    internal enum ToDoItemState
    {
        [Description("Active")]
        Active = 0,
        [Description("Completed")]
        Completed = 1
    }
}
