using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.InterfaceCommon
{
    public interface IGenericRepository<T> where T : class
    {
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> GetAllNoTrackingAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
        Task<PagedList<T>> GetAllIncludingPaginatedAsync(Expression<Func<T, bool>> filter = null, PaginationFilter paginationFilter = null, CancellationToken cancellationToken = default, params Expression<Func<T, object>>[] includeProperties);
        DbSet<T> EntitySet { get; }
        T Get(Expression<Func<T, bool>> expression);
        Task<T> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
        Task<IEnumerable<T>> GetAllSelect();
        IEnumerable<T> GetAll(PaginationFilter paginationFilter = null);
        IQueryable<T> GetAllQ(PaginationFilter paginationFilter = null);

        IEnumerable<T> GetAll(Expression<Func<T, bool>> expression, PaginationFilter paginationFilter = null);

        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);


        Task<IEnumerable<T>> GetAllIncludingAsync(Expression<Func<T, bool>> expression = null,
            PaginationFilter? paginationFilter = null, params Expression<Func<T, object>>[] includeProperties);
        Task<PagedList<T>> GetAllPaginatedAsync(Expression<Func<T, bool>> expression, PaginationFilter paginationFilter, CancellationToken cancellationToken = default);
        Task<PagedList<T>> GetAllPaginatedAsync(PaginationFilter paginationFilter, CancellationToken cancellationToken = default);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

        Task<T> GetIncludingAsync(Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] includeProperties);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken = default);
    }

}
