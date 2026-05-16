using ConsoleApp1.Infrastructure.DataAccess.Models;
using LinqToDB;
using LinqToDB.Data;

namespace ConsoleApp1.Infrastructure.DataAccess
{
    internal class ToDoDataContext : DataConnection
    {
        public ToDoDataContext(string connectionString) : base(ProviderName.PostgreSQL, connectionString)
        {
        }

        public ITable<ToDoUserModel> ToDoUsers => this.GetTable<ToDoUserModel>();
        public ITable<ToDoListModel> ToDoLists => this.GetTable<ToDoListModel>();
        public ITable<ToDoItemModel> ToDoItems => this.GetTable<ToDoItemModel>();
        public ITable<NotificationModel> Notifications => this.GetTable<NotificationModel>();
    }
}