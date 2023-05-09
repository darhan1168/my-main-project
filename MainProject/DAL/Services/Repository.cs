using Core;
using DAL.Abstraction.Interfaces;
using Newtonsoft.Json;

namespace DAL
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly string _filePath;

        public Repository(string? filePath = null)
        {
            _filePath = filePath ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{typeof(T).Name}.json");
            EnsureFileExists();
        }
        public List<T> GetAll(int pageNumber = 1, int pageSize = 10)
        {
            throw new NotImplementedException();
        }

        public T GetById(Guid id)
        {
            throw new NotImplementedException();
        }

        public T GetByPredicate(Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Add(T obj)
        {
            throw new NotImplementedException();
        }

        public void Update(Guid id, T updateObj)
        {
            throw new NotImplementedException();
        }

        public void Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        private void EnsureFileExists()
        {
            if (!File.Exists(_filePath))
            {
                WriteToFile(new List<T>());
            }
        }

        private void WriteToFile(List<T> items)
        {
            try
            {
                using StreamWriter file = File.CreateText(_filePath);
                using JsonTextWriter writer = new JsonTextWriter(file)
                {
                    Formatting = Formatting.Indented
                };

                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, items);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to write items to the file. Exception: {ex.Message}");
            }
        }
    }
}