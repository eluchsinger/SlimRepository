using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SlimRepository.Interfaces
{
    /// <summary>
    /// A repository to handle CRUD operations of entities.
    /// </summary>
    /// <typeparam name="T">The entity type to handle with this repository.</typeparam>
    public interface IRepository<T> where T : class
    {
        T GetById(int id);

        List<T> List(bool asNoTracking = false);

        List<T> List(ISpecification<T> specification, bool asNoTracking = false);

        List<T> List(Expression<Func<T, bool>> predicate, bool asNoTracking = false);

        T Add(T entity);

        void AddRange(IEnumerable<T> entities);

        void Delete(T entity);

        void Edit(T entity);
    }
}