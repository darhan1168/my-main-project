using Core;
using DAL.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace DAL;

public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly AppContext _dbContext;

    public async Task CommitAsync()
    {
        await _dbContext.SaveChangesAsync();
    }

    private bool disposed = false;

    protected virtual async Task Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                await _dbContext.DisposeAsync();
            }
        }
        this.disposed = true;
    }

    public async void Dispose()
    {
        await Dispose(true);
        GC.SuppressFinalize(this);
    }
}   