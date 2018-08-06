using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlimRepository.Interfaces;

namespace SlimRepository.EntityFrameworkCore
{
    public class AsyncRepository<T> : IAsyncRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public AsyncRepository(DbContext context)
        {
            Context = context;
        }

        public virtual Task<T> GetByIdAsync(int id)
        {
            return Context.Set<T>().FindAsync(id);
        }

        public virtual Task<List<T>> ListAsync(bool asNoTracking = false)
        {
            var query = Context.Set<T>().AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.ToListAsync();
        }

        public virtual Task<List<T>> ListAsync(ISpecification<T> specification, bool asNoTracking = false)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Context.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            var secondaryResult = specification.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            if (asNoTracking)
            {
                secondaryResult = secondaryResult.AsNoTracking();
            }

            return secondaryResult
                .Where(specification.Criteria)
                .ToListAsync();
        }

        public virtual Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            var query = Context.Set<T>().AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.Where(predicate)
                .ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await Context.Set<T>().AddAsync(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual Task AddRangeAsync(IEnumerable<T> entities)
        {
            return Context.Set<T>().AddRangeAsync(entities)
                .ContinueWith(addTask => Context.SaveChangesAsync());
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
    }
}