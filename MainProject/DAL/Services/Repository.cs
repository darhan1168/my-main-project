using System.Linq.Expressions;
using Core;
using DAL.Abstraction.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

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
        
        public IQueryable<T> GetAll()
        {
            return _dbSet;
        }

        public T GetById(Guid id)
        {
            return _dbSet.Where(o => o.Id.Equals(id)).FirstOrDefault();
        }

        public T GetByPredicate(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate).FirstOrDefault();
        }

        public void Add(T obj)
        {
            DbSet.Add(obj);
        }

        public void Update(Guid id, T updateObj)
        {
            var obj = GetById(id);
            obj = updateObj;
            DbSet.Update(obj);
        }

        public void Delete(Guid id)
        {
            DbSet.Remove(GetById(id));
        }
    }
}