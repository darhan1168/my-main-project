using System.Linq.Expressions;
using Core;

namespace DAL.Abstraction.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        IQueryable<T> GetAll();

        T GetById(Guid id);

        T GetByPredicate(Expression<Func<T, bool>> predicate);

        void Add(T obj);

        void Update(Guid id, T updateObj);

        void Delete(Guid id);
    }
}