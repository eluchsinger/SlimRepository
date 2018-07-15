using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SlimRepository.Interfaces
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        List<T> List();
        List<T> List(ISpecification<T> specification);
        List<T> List(Expression<Func<T, bool>> predicate);
        T Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Delete(T entity);
        void Edit(T entity);
    }
}
