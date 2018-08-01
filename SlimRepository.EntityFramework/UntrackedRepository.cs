using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using SlimRepository.Interfaces;

namespace SlimRepository.EntityFramework
{
    public class UntrackedRepository<T> : IUntrackedRepository<T> where T : class
    {
        protected readonly DbContext Context;

        public UntrackedRepository(DbContext context)
        {
            Context = context;
        }

        public virtual T GetById(int id)
        {
            return Context.Set<T>().Find(id);
        }

        public virtual List<T> List()
        {
            return Context.Set<T>()
                .AsNoTracking()
                .ToList();
        }

        public virtual List<T> List(ISpecification<T> specification)
        {
            var queryableResultWithIncludes = specification.Includes
                .Aggregate(Context.Set<T>().AsQueryable(),
                    (current, include) => current.Include(include));

            var secondaryResult = specification.IncludeStrings
                .Aggregate(queryableResultWithIncludes,
                    (current, include) => current.Include(include));

            return secondaryResult
                .Where(specification.Criteria)
                .AsNoTracking()
                .ToList();
        }

        public virtual List<T> List(Expression<Func<T, bool>> predicate)
        {
            return Context.Set<T>()
                .Where(predicate)
                .AsNoTracking()
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