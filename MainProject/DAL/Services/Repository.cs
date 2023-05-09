using Core;
using DAL.Abstraction.Interfaces;

namespace DAL
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
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
    }
}