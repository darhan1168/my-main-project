using System.Linq.Expressions;
using Core;

namespace UI.Interfaces;

public interface IConsoleManager<TEntity> where TEntity : BaseEntity
{
    void PerformOperations();

    TEntity GetById(Guid id);

    Task<TEntity> GetByPredicate(Expression<Func<TEntity, bool>> predicate);
    
    Task<List<TEntity>> GetListByPredicate(Expression<Func<TEntity, bool>> predicate);

    void Add(TEntity obj);

    void Update(Guid id, TEntity updateObj);

    void Delete(Guid id);
}