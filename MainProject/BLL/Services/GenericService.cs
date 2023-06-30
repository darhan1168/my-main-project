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
            _repository.Add(obj);
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
            _repository.Delete(id);
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
            return _repository.GetById(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get {typeof(T).Name} by Id {id}. Exception: {ex.Message}");
        }
    }

    public List<T> GetAll()
    {
        try
        {
            return _repository.GetAll().ToList();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get all {typeof(T).Name}s. Exception: {ex.Message}");
        }
    }

    public T GetByPredicate(Expression<Func<T, bool>> predicate)
    {
        try
        {
            return _repository.GetByPredicate(predicate);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get by predicate {typeof(T).Name}s. Exception: {ex.Message}");
        }
    }

    public void Update(Guid id, T obj)
    {
        try
        {
            _repository.Update(id, obj);
            _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to update {typeof(T).Name} with Id {id}. Exception: {ex.Message}");
        }
    }
}