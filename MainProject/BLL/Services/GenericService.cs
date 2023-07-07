using System.Linq.Expressions;
using BLL.Abstraction.Interfaces;
using Core;
using DAL.Abstraction.Interfaces;

namespace BLL;

public class GenericService<T> : IGenericService<T> where T : BaseEntity
{
    private readonly IRepository<T> _repository;
    private readonly IUnitOfWork _unitOfWork;

    protected GenericService(IRepository<T> repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public void Add(T obj)
    {
        try
        {
            _repository.AddAsync(obj);
            _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to add {typeof(T).Name}. Exception: {ex.Message}");
        }
    }

    public void Delete(Guid id)
    {
        try
        {
            _repository.DeleteAsync(id);
            _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to delete {typeof(T).Name} with Id {id}. Exception: {ex.Message}");
        }
    }

    public T GetById(Guid id)
    {
        try
        {
            var task = _repository.GetByIdAsync(id);
            task.Wait(); 

            return task.Result;
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

    public void Update(Guid id, T obj)
    {
        try
        {
            _repository.UpdateAsync(id, obj);
            _unitOfWork.CommitAsync();
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