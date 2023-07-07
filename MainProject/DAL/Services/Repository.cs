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
        private readonly DbFactory _dbFactory;
        private DbSet<T> _dbSet;

        protected DbSet<T> DbSet
        {
            get => _dbSet ?? (_dbSet = _dbFactory.DbContext.Set<T>());
        }

        public Repository(DbFactory dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<T> GetByPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.Where(predicate).FirstOrDefaultAsync();
        }
        
        public async Task<List<T>> GetListByPredicateAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public async Task AddAsync(T obj)
        {
            await DbSet.AddAsync(obj);
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetByIdAsync(id);
            
            DbSet.Remove(entity);
        }

        public async Task<List<T>> ListAsync(Expression<Func<T, bool>> expression)
        {
            return await DbSet.Where(expression).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await DbSet.Where(e => e.Id.Equals(id)).FirstOrDefaultAsync();
        }

        public async Task UpdateAsync(Guid id, T updateObj)
        {
            var entity = await GetByIdAsync(id);
            entity = updateObj;
            
            DbSet.Update(entity);
        }
    }
}