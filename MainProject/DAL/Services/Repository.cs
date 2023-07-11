using System.Linq.Expressions;
using Core;
using DAL.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Task = System.Threading.Tasks.Task;

namespace DAL
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppContext _dbContext;
        private DbSet<T> _dbSet;
        
        public Repository(AppContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).FirstOrDefaultAsync();
        }
        
        public async Task<List<T>> GetListByPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T obj)
        {
            await _dbSet.AddAsync(obj);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            
            _dbSet.Remove(entity);
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<T>> ListAsync(Expression<Func<T, bool>> expression)
        {
            return await _dbSet.Where(expression).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbSet.Where(e => e.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Guid id, T updateObj)
        {
            var entity = await GetByIdAsync(id);
            entity = updateObj;
            
            _dbSet.Update(entity);
            
            await _dbContext.SaveChangesAsync();
        }
    }
}