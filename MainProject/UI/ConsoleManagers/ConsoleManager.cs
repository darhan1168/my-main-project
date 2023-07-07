using System.Linq.Expressions;
using BLL.Abstraction.Interfaces;
using Core;
using UI.Interfaces;

namespace UI.ConsoleManagers;

public abstract class ConsoleManager<TService, TEntity> 
    where TEntity : BaseEntity
    where TService : IGenericService<TEntity>
{
    protected readonly TService _service;

    protected ConsoleManager(TService service)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public abstract void PerformOperations();

    public virtual TEntity GetById(Guid id)
    {
        try
        {
            return _service.GetById(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetById: {ex.Message}");
                
            return null;
        }
    }

    public async Task<TEntity> GetByPredicate(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await _service.GetByPredicate(predicate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByPredicate: {ex.Message}");
                
            return null;
        }
    }
    
    public virtual async Task<List<TEntity>> GetListByPredicate(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            return await _service.GetListByPredicate(predicate);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get by predicate. Exception: {ex.Message}", ex);
        }
    }

    public virtual void Add(TEntity obj)
    {
        if (obj == null)
        {
            throw new ArgumentNullException(nameof(obj));
        }
        
        try
        {
            _service.Add(obj);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Add: {ex.Message}");
        }
    }

    public virtual void Update(Guid id, TEntity updateObj)
    {
        if (updateObj == null)
        {
            throw new ArgumentNullException(nameof(updateObj));
        }
        
        try
        {
            _service.Update(id, updateObj);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Update: {ex.Message}");
        }
    }

    public virtual void Delete(Guid id)
    {
        try
        {
            _service.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in Delete: {ex.Message}");
        }
    }
}