using System.Linq.Expressions;
using Core;
using Task = System.Threading.Tasks.Task;

namespace BLL.Abstraction.Interfaces;

public interface IGenericService<T> where T : BaseEntity
{
    Task Add(T obj);
    
    Task Delete(Guid id);

    Task<T> GetById(Guid id);

    Task<T> GetByPredicate(Expression<Func<T, bool>> predicate);
    
    Task<List<T>> GetListByPredicate(Expression<Func<T, bool>> predicate);

    Task Update(Guid id, T obj);
}