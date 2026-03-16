using System.Text.Json;

namespace ConsoleApp1.Helpers
{
    internal static class FileLinkIndex
    {
        private static readonly SemaphoreSlim _gate = new SemaphoreSlim(1, 1);
        private static string _storagePath;
        private static Dictionary<string, string> _listIndexes = new Dictionary<string, string>();
        private static Dictionary<string, string> _taskIndexes = new Dictionary<string, string>();
        public static void Initialize(string path)
        {
            if (!string.IsNullOrWhiteSpace(_storagePath))
                throw new InvalidOperationException("Путь уже задан");
            _storagePath = path;
            UpdateAllIndexes();
        }
        private static void UpdateAllIndexes()
        {
            if(!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
            UpdateTaskIndexes();
            UpdateListIndexes();
        }

        private static void UpdateListIndexes()
        {
            string json = string.Empty;
            _listIndexes.Clear();
            string[] users = Directory.GetFiles(_storagePath, "*.json", SearchOption.TopDirectoryOnly).Where(name => name.LastIndexOf("Indexes.json") == -1).ToArray();
            for (int i = 0; i < users.Length; i++)
            {//Получаем папки пользователей, в которых ищем директорию Lists и после получаем список файлов json. В итоге собираем пары лист+юзер
                string pathToTasksUser = users[i].Remove(users[i].LastIndexOf(".json"));
                string? ListsDirectory = Directory.GetDirectories(pathToTasksUser).FirstOrDefault(dir => dir.LastIndexOf("Lists") != -1);
                if (ListsDirectory == null)
                    continue;
                foreach (string pathToList in Directory.GetFiles(ListsDirectory))
                {
                    _listIndexes[Path.GetFileNameWithoutExtension(pathToList)] = Path.GetFileNameWithoutExtension(users[i]);
                }
            }
            json = JsonSerializer.Serialize(_taskIndexes);
            File.WriteAllText(Path.Combine(_storagePath, "TaskListIndexes.json"), json);
        }

        private static void UpdateTaskIndexes()
        {
            string json = string.Empty;
            _taskIndexes.Clear();
            string[] users = Directory.GetFiles(_storagePath, "*.json", SearchOption.TopDirectoryOnly).Where(name => name.LastIndexOf("Indexes.json") == -1).ToArray();
            for (int i = 0; i < users.Length; i++)
            {//ищем задачи по папка пользователей. Учитываем, что адрес без ".json" т.е. нужны папки.
                string pathToTaskUser = users[i].Remove(users[i].LastIndexOf(".json"));
                string[] tasks = Directory.GetFiles(pathToTaskUser);
                for (int j = 0; j < tasks.Length; j++)
                {
                    string taskId = Path.GetFileNameWithoutExtension(tasks[j]);
                    string userId = Path.GetFileNameWithoutExtension(users[i]);
                    _taskIndexes.Add(taskId, userId);
                }
            }
            json = JsonSerializer.Serialize(_taskIndexes);
            File.WriteAllText(Path.Combine(_storagePath, "TaskIndexes.json"), json);
        }
        public static async Task AddTaskIndex(string pathToTask, string pathToUser)
        {
            await _gate.WaitAsync();
            try
            {
                _taskIndexes.Add(pathToTask, pathToUser);
                string json = JsonSerializer.Serialize(_taskIndexes);
                File.WriteAllText(Path.Combine(_storagePath, "TaskIndexes.json"), json);
            }
            finally
            {
                _gate.Release();
            }
        }
        public static async Task RemoveTaskIndex(string taskId)
        {
            await _gate.WaitAsync();
            try
            {
                _taskIndexes.Remove(taskId);
                string json = JsonSerializer.Serialize(_taskIndexes);
                File.WriteAllText(Path.Combine(_storagePath, "TaskIndexes.json"), json);
            }
            finally
            {
                _gate.Release();
            }
        }

        public static async Task AddTaskListIndex(string pathToTaskList, string pathToUser)
        {
            await _gate.WaitAsync();
            try
            {
                _listIndexes.Add(pathToTaskList, pathToUser);
                string json = JsonSerializer.Serialize(_taskIndexes);
                File.WriteAllText(Path.Combine(_storagePath, "TaskListIndexes.json"), json);
            }
            finally
            {
                _gate.Release();
            }
        }
        public static async Task RemoveTaskListIndex(string listId)
        {
            await _gate.WaitAsync();
            try
            {
                _listIndexes.Remove(listId);
                string json = JsonSerializer.Serialize(_taskIndexes);
                File.WriteAllText(Path.Combine(_storagePath, "TaskListIndexes.json"), json);
            }
            finally
            {
                _gate.Release();
            }
        }
        public static async Task<IReadOnlyDictionary<string,string>> GetListIndexes()
        {
            return _listIndexes;
        }
        public static async Task<IReadOnlyDictionary<string, string>> GetTaskIndexes()
        {
            return _taskIndexes;
        }
    }
}