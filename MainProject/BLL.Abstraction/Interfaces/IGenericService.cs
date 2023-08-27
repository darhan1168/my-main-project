using System.Linq.Expressions;
using Core;
using Task = System.Threading.Tasks.Task;

namespace BLL.Abstraction.Interfaces;

public interface IGenericService<T> where T : BaseEntity
{
    Task Add(T obj);
    
    Task Delete(Guid id);

    Task<T> GetById(Guid id);

    Task<T> GetById(Guid id, string includeProperties);

    Task<T> GetByPredicate(Expression<Func<T, bool>> predicate);
    
    Task<List<T>> GetListByPredicate(Expression<Func<T, bool>> predicate);
    
    Task<IEnumerable<T>> GetList(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
        string includeProperties = "");

    Task Update(Guid id, T obj);
}