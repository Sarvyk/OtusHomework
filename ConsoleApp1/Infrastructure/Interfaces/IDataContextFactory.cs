using LinqToDB.Data;

namespace ConsoleApp1.Infrastructure.Interfaces
{
    internal interface IDataContextFactory<TDataContext> where TDataContext : DataConnection
    {
        TDataContext CreateDataContext();
    }
}
