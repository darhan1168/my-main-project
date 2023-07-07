using System.Linq.Expressions;
using Core;

namespace BLL.Abstraction.Interfaces;

public interface IGenericService<T> where T : BaseEntity
{
    void Add(T obj);
    
    void Delete(Guid id);

    T GetById(Guid id);

    Task<T> GetByPredicate(Expression<Func<T, bool>> predicate);
    
    Task<List<T>> GetListByPredicate(Expression<Func<T, bool>> predicate);

    void Update(Guid id, T obj);
}