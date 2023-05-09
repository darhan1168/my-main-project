using Core;

namespace DAL.Abstraction.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        List<T> GetAll(int pageNumber = 1, int pageSize = 10);

        T GetById(Guid id);

        T GetByPredicate(Func<T, bool> predicate);

        void Add(T obj);

        void Update(Guid id, T updateObj);

        void Delete(Guid id);
    }
}