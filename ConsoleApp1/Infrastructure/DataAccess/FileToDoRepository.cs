using ConsoleApp1.DataAccess;
using ConsoleApp1.Entities;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace ConsoleApp1.Infrastructure.DataAccess
{
    internal class FileToDoRepository : IToDoRepository
    {
        private readonly string _storagePath;
        private Dictionary<string, string> _indexes = new Dictionary<string, string>();
        public FileToDoRepository(string storagePath)
        {
            _storagePath = storagePath;
            if (!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
            if (!File.Exists(Path.Combine(storagePath, "indexes.json")))
                File.Create(Path.Combine(storagePath, "indexes.json")).Close();

            UpdateIndexes(IndexOperation.UpdateAll);
        }
        public async Task AddAsync(ToDoItem item, CancellationToken ct)
        {
            string pathToUser = Path.Combine(_storagePath, $"{item.User.UserId}.json");
            string pathToTask = Path.Combine(_storagePath, $"{item.User.UserId}", $"{item.id}.json");
            using (FileStream stream = File.Create(pathToTask))
            {
                await JsonSerializer.SerializeAsync(stream, item, cancellationToken: ct);
            }
            _indexes.Add(item.id.ToString(),item.User.UserId.ToString());
            UpdateIndexes(IndexOperation.Update);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct)
        {
            if (!_indexes.ContainsKey(id.ToString()))
                throw new ArgumentException("Такой задачи нет.");
            string path = Path.Combine(_storagePath, _indexes[id.ToString()], $"{id.ToString()}.json");
            File.Delete(path);
            _indexes.Remove(id.ToString());
            UpdateIndexes(IndexOperation.Update);
        }

        public async Task<int> CountActiveAsync(Guid userId, CancellationToken ct)
        {//этот метод в первую очередь
            return (await GetTasks(userId, ct)).Count(x => x.State == ToDoItemState.Active);
        }

        public async Task<bool> ExistsByNameAsync(Guid userId, string name, CancellationToken ct)
        {
            return (await GetTasks(userId, ct)).Any(x => x.User.UserId == userId && x.Name == name);
        }
        public async Task<IReadOnlyList<ToDoItem>> FindAsync(Guid userId, Func<ToDoItem, bool> predicate, CancellationToken ct)
        {
            var items = (await GetTasks(userId, ct)).Where(x => x.User.UserId == userId).ToList();
            return items.Where(x => predicate(x)).ToList();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return (await GetTasks(userId, ct)).Where(x => x.User.UserId == userId && x.State == ToDoItemState.Active).ToList();
        }

        public async Task<IReadOnlyList<ToDoItem>> GetAllByUserIdAsync(Guid userId, CancellationToken ct)
        {
            return (await GetTasks(userId, ct)).Where(x => x.User.UserId == userId).ToList();
        }

        public async Task<ToDoItem?> GetAsync(Guid userId, Guid id, CancellationToken ct)
        {
            return (await GetTasks(userId, ct)).FirstOrDefault(x => x.id == id);
        }

        public async Task UpdateAsync(ToDoItem item, CancellationToken ct)
        {
            if(!_indexes.ContainsKey(item.id.ToString()))
                throw new ArgumentException("Такой задачи нет");
            string pathToTask = Path.Combine(_storagePath, item.User.UserId.ToString(), $"{item.id.ToString()}.json");
            ToDoItem itemChanging = null;
            using(FileStream stream = new FileStream(pathToTask, FileMode.Open, FileAccess.Read))
            {
                itemChanging = await JsonSerializer.DeserializeAsync<ToDoItem>(stream, cancellationToken: ct);
                itemChanging.State = ToDoItemState.Completed;
            }
            using (FileStream stream = new FileStream(pathToTask, FileMode.Create, FileAccess.Write))
            {
                await JsonSerializer.SerializeAsync(stream, itemChanging, cancellationToken: ct);
            }
        }

        private async Task<List<ToDoItem>> GetTasks(Guid userId, CancellationToken ct)
        {
            List<ToDoItem> result = new List<ToDoItem>();
            string[] files = Directory.GetFiles(Path.Combine(_storagePath, userId.ToString()));
            for (int i = 0; i < files.Length; i++)
            {
                using (FileStream stream = File.OpenRead(files[i]))
                {
                    result.Add(await JsonSerializer.DeserializeAsync<ToDoItem>(stream, cancellationToken: ct));
                }
            }
            return result;
        }

        private void UpdateIndexes(IndexOperation operation)
        {
            string json = string.Empty;
            switch (operation)
            {
                case IndexOperation.Update:
                    json = JsonSerializer.Serialize(_indexes);
                    File.WriteAllText(Path.Combine(_storagePath, "indexes.json"), json);
                    break;
                case IndexOperation.UpdateAll:
                    _indexes.Clear();
                    string[] users = Directory.GetFiles(_storagePath, "*.json", SearchOption.TopDirectoryOnly).Where(name => name.LastIndexOf("indexes.json") == -1).ToArray();
                    for (int i = 0; i < users.Length; i++)
                    {//ищем задачи по папка пользователей. Учитываем, что адрес без ".json" т.е. нужны папки.
                        string pathToTaskUser = users[i].Remove(users[i].LastIndexOf(".json"));
                        string[] tasks = Directory.GetFiles(pathToTaskUser);
                        for (int j = 0; j < tasks.Length; j++)
                        {
                            string taskId = Path.GetFileNameWithoutExtension(tasks[j]);
                            string userId = Path.GetFileNameWithoutExtension(users[i]);
                            _indexes.Add(taskId, userId);
                        }
                    }
                    json = JsonSerializer.Serialize(_indexes);
                    File.WriteAllText(Path.Combine(_storagePath, "indexes.json"), json);
                    break;
            }
        }

    }
}