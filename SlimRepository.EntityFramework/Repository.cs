using SlimRepository.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace SlimRepository.EntityFramework
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context;
        }

        public virtual T GetById(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public virtual List<T> List(bool asNoTracking = false)
        {
            var query = Context.Set<T>().AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query.ToList();
        }

        public virtual List<T> List(ISpecification<T> specification, bool asNoTracking = false)
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
                .ToList();
        }

        public virtual List<T> List(Expression<Func<T, bool>> predicate, bool asNoTracking = false)
        {
            var query = Context.Set<T>().AsQueryable();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return query
                .Where(predicate)
                .ToList();
        }

        public virtual T Add(T entity)
        {
            Context.Set<T>().Add(entity);
            Context.SaveChanges();
            return entity;
        }

        public virtual void AddRange(IEnumerable<T> entities)
        {
            Context.Set<T>().AddRange(entities);
            Context.SaveChanges();
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
    }
}