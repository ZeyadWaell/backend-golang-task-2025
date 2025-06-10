using EasyOrder.Application.Contracts.Filters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Interfaces.Main
{
    public interface IGenericRepository<T> where T : class
    {

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        T Get(Expression<Func<T, bool>> predicate);
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<T> GetIncludingAsync(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties);

        IEnumerable<T> GetAll(PaginationFilter paginationFilter = null);
        IQueryable<T> GetAllQ(PaginationFilter paginationFilter = null);
        IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, PaginationFilter paginationFilter = null);

        Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default);
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);
        Task<IEnumerable<T>> GetAllNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default);

        Task<PagedList<T>> GetAllPaginatedAsync(PaginationFilter paginationFilter, CancellationToken ct = default);
        Task<PagedList<T>> GetAllPaginatedAsync(Expression<Func<T, bool>> predicate,
                                                PaginationFilter paginationFilter,
                                                CancellationToken ct = default);

        Task<PagedList<T>> GetAllIncludingPaginatedAsync(
            Expression<Func<T, bool>> filter = null,
            PaginationFilter paginationFilter = null,
            CancellationToken ct = default,
            params Expression<Func<T, object>>[] includeProperties);

        Task<IEnumerable<T>> GetAllIncludingAsync(
            Expression<Func<T, bool>> predicate = null,
            PaginationFilter paginationFilter = null,
            params Expression<Func<T, object>>[] includeProperties);

        Task<int> CountAsync(Expression<Func<T, bool>> filter = null, CancellationToken ct = default);

        // >>> COMMAND (writes go to WriteDbContext) <<<

        void Add(T entity);
        Task AddAsync(T entity, CancellationToken ct = default);
        void AddRange(IEnumerable<T> entities);
        Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default);

        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);

        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);

        Task<int> SaveChangesAsync(CancellationToken ct = default);
    }

}
