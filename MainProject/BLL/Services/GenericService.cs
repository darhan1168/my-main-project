using System.Linq.Expressions;
using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;
using Task = System.Threading.Tasks.Task;

namespace BLL;

public class GenericService<T> : IGenericService<T> where T : BaseEntity
{
    private readonly IRepository<T> _repository;

    protected GenericService(IRepository<T> repository)
    {
        _repository = repository;
    }

    public async Task Add(T obj)
    {
        try
        {
            await _repository.AddAsync(obj);
            //await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to add {typeof(T).Name}. Exception: {ex.Message}");
        }
    }

    public async Task Delete(Guid id)
    {
        try
        {
            await _repository.DeleteAsync(id);
            //await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete {typeof(T).Name} with Id {id}. Exception: {ex.Message}");
        }
    }

    public async Task<T> GetById(Guid id)
    {
        try
        {
            return await _repository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get {typeof(T).Name} by Id {id}. Exception: {ex.Message}");
        }
    }

    public async Task<T> GetByPredicate(Expression<Func<T, bool>> predicate)
    {
        try
        {
            return await _repository.GetByPredicateAsync(predicate);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get {typeof(T).Name} by predicate. Exception: {ex.Message}", ex);
        }
    }

    public async Task Update(Guid id, T obj)
    {
        try
        {
            await _repository.UpdateAsync(id, obj);
            //await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {typeof(T).Name} with Id {id}. Exception: {ex.Message}");
        }
    }
    
    public async Task<List<T>> GetListByPredicate(Expression<Func<T, bool>> predicate)
    {
        try
        {
            return await _repository.GetListByPredicateAsync(predicate);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get {typeof(T).Name} by predicate. Exception: {ex.Message}", ex);
        }
    }
}