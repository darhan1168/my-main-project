using Microsoft.EntityFrameworkCore;

namespace DAL.Abstraction.Interfaces;

public interface IUnitOfWork
{
    Task CommitAsync();
}