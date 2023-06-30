namespace DAL.Abstraction.Interfaces;

public interface IUnitOfWork
{
    Task<int> CommitAsync();
}