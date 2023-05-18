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

    public virtual IEnumerable<TEntity> GetAll()
    {
        try
        {
            return _service.GetAll();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetAll: {ex.Message}");
                
            return Enumerable.Empty<TEntity>();
        }
    }

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

    public virtual TEntity GetByPredicate(Func<TEntity, bool> predicate)
    {
        try
        {
            return _service.GetByPredicate(predicate);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetByPredicate: {ex.Message}");
                
            return null;
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