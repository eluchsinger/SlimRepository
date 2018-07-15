using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SlimRepository.Interfaces
{
    public interface IAsyncRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<List<T>> ListAsync();
        Task<List<T>> ListAsync(ISpecification<T> specification);
        Task<List<T>> ListAsync(Expression<Func<T, bool>> predicate);
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task EditAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
