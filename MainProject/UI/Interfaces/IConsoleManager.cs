using System.Linq.Expressions;
using Core;
using Task = System.Threading.Tasks.Task;

namespace UI.Interfaces;

public interface IConsoleManager<TEntity> where TEntity : BaseEntity
{
    Task PerformOperations();

    Task<TEntity> GetById(Guid id);

    Task<TEntity> GetByPredicate(Expression<Func<TEntity, bool>> predicate);
    
    Task<List<TEntity>> GetListByPredicate(Expression<Func<TEntity, bool>> predicate);

    Task Add(TEntity obj);

    Task Update(Guid id, TEntity updateObj);

    Task Delete(Guid id);
}