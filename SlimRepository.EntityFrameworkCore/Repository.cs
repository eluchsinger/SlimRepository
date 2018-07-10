using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlimRepository.Interfaces;

namespace SlimRepository.EntityFrameworkCore
{
    public class Repository<T> : IRepository<T>, IAsyncRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        #region Synchronous Methods

        public virtual T GetById(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public virtual List<T> List()
        {
            return Context.Set<T>().ToList();
        }

        public List<T> List(ISpecification<T> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Context.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            var secondaryResult = specification.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            return secondaryResult
                .Where(specification.Criteria)
                .ToList();
        }

        public virtual List<T> List(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>()
                .Where(predicate)
                .ToList();
        }

        public virtual T Add(T entity)
        {
            Context.Set<T>().Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public virtual void Delete(T entity)
        {
            Context.Set<T>().Remove(entity);
            Context.SaveChanges();
        }

        public virtual void Edit(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        #endregion Synchronous Methods

        #region Asynchronous Methods

        public virtual Task<T> GetByIdAsync(int id)
        {
            return Context.Set<T>().FindAsync(id);
        }

        public virtual Task<List<T>> ListAsync()
        {
            return Context.Set<T>().ToListAsync();
        }

        public virtual Task<List<T>> ListAsync(ISpecification<T> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Context.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            var secondaryResult = specification.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            return secondaryResult
                .Where(specification.Criteria)
                .ToListAsync();
        }

        public virtual Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>()
                .Where(predicate)
                .ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual Task EditAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return Context.SaveChangesAsync();
        }

        public virtual Task DeleteAsync(T entity)
        {
            Context.Set<T>().Remove(entity);
            return Context.SaveChangesAsync();
        }

        #endregion Asynchronous Methods
    }
}
