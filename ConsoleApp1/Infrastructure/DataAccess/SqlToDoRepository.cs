using LinqToDB;
using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Entities.Enums;
using ConsoleApp1.Core.Interfaces.DataAccess;
using ConsoleApp1.Infrastructure.Interfaces;

namespace ConsoleApp1.Infrastructure.DataAccess
{
    internal class SqlToDoRepository : IToDoRepository
    {
        private readonly IDataContextFactory<ToDoDataContext> _factory;

        public SqlToDoRepository(IDataContextFactory<ToDoDataContext> factory)
        {
            _factory = factory;
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var userModel = await dbContext.ToDoUsers.FirstAsync(u => u.ExternalId == userId, ct);

            if (userModel == null)
                return Array.Empty<ToDoItem>();

            var models = await AsyncExtensions.ToListAsync(dbContext.ToDoItems
                .Where(i => i.UserId == userModel.Id)
                .LoadWith(i => i.User)
                .LoadWith(i => i.List),ct);
            return models.Select(ModelMapper.MapFromModel).ToList();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var userModel = await dbContext.ToDoUsers.FirstAsync(u => u.ExternalId == userId, ct);

            if (userModel == null)
                return Array.Empty<ToDoItem>();

            var models = await AsyncExtensions.ToListAsync(dbContext.ToDoItems
                .Where(i => i.UserId == userModel.Id && i.ItemState == (int)ToDoItemState.Active)
                .LoadWith(i => i.User)
                .LoadWith(i => i.List),ct);

            return models.Select(ModelMapper.MapFromModel).ToList();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetCompletedByUserIdAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var userModel = await dbContext.ToDoUsers.FirstAsync(u => u.ExternalId == userId, ct);

            if (userModel == null)
                return Array.Empty<ToDoItem>();

            var models = await AsyncExtensions.ToListAsync(dbContext.ToDoItems
                .Where(i => i.UserId == userModel.Id && i.ItemState == (int)ToDoItemState.Completed)
                .LoadWith(i => i.User)
                .LoadWith(i => i.List),ct);

            return models.Select(ModelMapper.MapFromModel).ToList();
        }

        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var items = await GetAllByUserIdAsync(userId, ct);
            return items.Where(predicate).ToList();
        }

        public async Task<ToDoItem?> GetAsync(Guid id, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var model = await AsyncExtensions.FirstOrDefaultAsync(dbContext.ToDoItems
                .LoadWith(i => i.User)
                .LoadWith(i => i.List),
                i => i.ExternalId == id, ct);

            return model != null ? ModelMapper.MapFromModel(model) : null;
        }

        public async Task AddAsync(ToDoItem item, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();
            var model = ModelMapper.MapToModel(item);
            await dbContext.InsertAsync(model, token: ct);
        }

        public async Task UpdateAsync(ToDoItem item, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var model = await dbContext.ToDoItems
                .FirstOrDefaultAsync(i => i.ExternalId == item.Id, ct);

            if (model == null)
                throw new ArgumentException("Такой задачи нет");

            model.ItemState = (int)ToDoItemState.Completed;
            model.StateChangedAt = DateTime.UtcNow;
            await dbContext.UpdateAsync(model, token: ct);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var model = await dbContext.ToDoItems
                .FirstOrDefaultAsync(i => i.ExternalId == id, ct);

            if (model == null)
                throw new ArgumentException("Такой задачи нет");

            await dbContext.DeleteAsync(model, token: ct);
        }

        public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var userModel = await dbContext.ToDoUsers.FirstAsync(u => u.ExternalId == userId, ct);

            if (userModel == null)
                return false;

            return await dbContext.ToDoItems
                .AnyAsync(i => i.UserId == userModel.Id && i.ItemName == name, ct);
        }

        public async Task<int> CountActiveAsync(Guid userId, CancellationToken ct)
        {
            using var dbContext = _factory.CreateDataContext();

            var userModel = await dbContext.ToDoUsers.FirstAsync(u => u.ExternalId == userId, ct);

            if (userModel == null)
                return 0;

            return await dbContext.ToDoItems
                .CountAsync(i => i.UserId == userModel.Id && i.ItemState == (int)ToDoItemState.Active, ct);
        }
    }
}