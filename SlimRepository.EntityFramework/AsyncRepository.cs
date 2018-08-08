﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SlimRepository.Interfaces;

namespace SlimRepository.EntityFramework
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

        public virtual Task<List<T>> ListAsync()
        {
            return Context.Set<T>()
                .AsNoTracking()
                .ToListAsync();
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
                .AsNoTracking()
                .Where(specification.Criteria)
                .ToListAsync();
        }

        public virtual Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>()
                .AsNoTracking()
                .Where(predicate)
                .ToListAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            Context.Set<T>().Add(entity);
            await Context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
            await Context.SaveChangesAsync();
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