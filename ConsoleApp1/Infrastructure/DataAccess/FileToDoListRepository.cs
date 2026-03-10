using ConsoleApp1.Core.Entities;
using ConsoleApp1.Core.Entities.Enums;
using ConsoleApp1.Core.Exceptions;
using ConsoleApp1.Core.Interfaces.DataAccess;
using ConsoleApp1.Helpers;
using System.Text.Json;
using Telegram.Bot.Types;

namespace ConsoleApp1.Infrastructure.DataAccess
{
    internal class FileToDoListRepository : IToDoListRepository
    {
        private readonly string _storagePath;
        public FileToDoListRepository(string storagePath)
        {
            _storagePath = storagePath;
            if(!Directory.Exists(storagePath))
                Directory.CreateDirectory(storagePath);
        }
        public async Task Add(ToDoList list, CancellationToken ct)
        {
            string pathToLists = Path.Combine(_storagePath, list.User.UserId.ToString(), "Lists");
            if (!Directory.Exists(pathToLists))
            {
                Directory.CreateDirectory(pathToLists);
            }
            string pathToList = Path.Combine(pathToLists, $"{list.Id.ToString()}.json");
            using (FileStream stream = File.Create(pathToList))
            {
                await JsonSerializer.SerializeAsync(stream, list, cancellationToken: ct);
            }//добавим связку индекса id листа и userId. Поиск будет дотаточно простым т.к. по id пользователя легко можно найти его папку Lists
            await FileLinkIndex.AddTaskListIndex(list.Id.ToString(), list.User.UserId.ToString());
        }

        public async Task Delete(Guid id, CancellationToken ct)
        {
            if(!(await FileLinkIndex.GetListIndexes()).ContainsKey(id.ToString()))
            {
                throw new ArgumentException("Такого листа не существует");
            }
            string pathToList = Path.Combine(_storagePath, (await FileLinkIndex.GetListIndexes())[id.ToString()], "Lists", $"{id.ToString()}.json");
            File.Delete(pathToList);
            await FileLinkIndex.RemoveTaskListIndex(id.ToString());
        }

        public async Task<bool> ExistsByName(Guid userId, string name, CancellationToken ct)
        {
            string pathToLists = Path.Combine(_storagePath, userId.ToString(), "Lists");
            if (!Directory.Exists(pathToLists))
            {//создаём папку и выходим сразу т.к. проверять всё равно нечего ещё.
                Directory.CreateDirectory(pathToLists);
                return false;
            }
            string[] paths = Directory.GetFiles(pathToLists);
            foreach (string path in paths)
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    ToDoList list = await JsonSerializer.DeserializeAsync<ToDoList>(stream, cancellationToken: ct);
                    if (list.Name == name)
                        return true;
                }
            }
            return false;
        }

        public async Task<ToDoList?> Get(Guid id, CancellationToken ct)
        {
            if (!(await FileLinkIndex.GetListIndexes()).ContainsKey(id.ToString()))
                throw new ArgumentException("Такого листа не существует");
            string pathToList = Path.Combine(_storagePath, (await FileLinkIndex.GetListIndexes())[id.ToString()], "Lists", $"{id.ToString()}.json");
            ToDoList? list;
            using (FileStream stream = File.OpenRead(pathToList))
            {
                list = await JsonSerializer.DeserializeAsync<ToDoList>(stream, cancellationToken: ct);
            }
            return list;
        }

        public async Task<IReadOnlyList<ToDoList>> GetByUserId(Guid userId, CancellationToken ct)
        {
            List<ToDoList> lists = new List<ToDoList>();
            foreach(KeyValuePair<string,string> keyValue in (await FileLinkIndex.GetListIndexes()))
            {
                if (Path.GetFileNameWithoutExtension(keyValue.Value) != userId.ToString())
                {
                    continue;
                }
                string pathToList = Path.Combine(_storagePath, (await FileLinkIndex.GetListIndexes())[keyValue.Key], "Lists", $"{keyValue.Key}.json");
                using (FileStream stream = File.OpenRead(pathToList))
                {
                    ToDoList list = await JsonSerializer.DeserializeAsync<ToDoList>(stream, cancellationToken: ct);
                    lists.Add(list);
                }
            }
            return lists;
        }
    }
}
