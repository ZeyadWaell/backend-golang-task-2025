using EasyOrder.Application.Contracts.InterfaceCommon;
using EasyOrder.Domain.Common;
using EasyOrder.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.Persistence.Repositories.Main
{

    public class GenericRepository<T> : IGenericRepository<T> where T : Base
    {
        protected readonly IAppDbContext _context;
        protected readonly DbSet<T> EntitySet;

        public GenericRepository(IAppDbContext context)
        {
            _context = context;
            EntitySet = context.Set<T>();
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
            => await EntitySet.AnyAsync(predicate);

        public void Add(T entity)
            => _context.Set<T>().Add(entity);

        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
            => await _context.Set<T>().AddAsync(entity, cancellationToken);

        public void AddRange(IEnumerable<T> entities)
            => _context.Set<T>().AddRange(entities);

        public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            => await _context.Set<T>().AddRangeAsync(entities, cancellationToken);

        public T Get(Expression<Func<T, bool>> predicate)
            => EntitySet.FirstOrDefault(predicate);

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
            => await EntitySet.FirstOrDefaultAsync(predicate, cancellationToken);

        public Task<T> GetIncludingAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            IQueryable<T> query = EntitySet;
            foreach (var include in includeProperties)
                query = query.Include(include);

            return query.FirstOrDefaultAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> filter = null)
            => filter == null
                ? await EntitySet.CountAsync()
                : await EntitySet.CountAsync(filter);

        public IEnumerable<T> GetAll(PaginationFilter paginationFilter = null)
        {
            var query = EntitySet.OrderByDescending(x => x.CreatedOn).AsQueryable();
            if (paginationFilter != null)
                query = query
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize);

            return query.ToList();
        }

        public IQueryable<T> GetAllQ(PaginationFilter paginationFilter = null)
        {
            var query = EntitySet.OrderByDescending(x => x.CreatedOn);
            if (paginationFilter != null)
                query = query
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize);

            return query;
        }

        public IEnumerable<T> GetAll(
            Expression<Func<T, bool>> predicate,
            PaginationFilter paginationFilter = null
        )
        {
            var query = EntitySet.Where(predicate).OrderByDescending(x => x.CreatedOn);
            if (paginationFilter != null)
                query = query
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize);

            return query.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
            => await EntitySet.OrderByDescending(x => x.CreatedOn).ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetAllAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            => await EntitySet
                .Where(predicate)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<T>> GetAllNoTrackingAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default
        )
            => await EntitySet
                .AsNoTracking()
                .Where(predicate)
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync(cancellationToken);

        public async Task<PagedList<T>> GetAllPaginatedAsync(
            PaginationFilter paginationFilter,
            CancellationToken cancellationToken = default
        )
            => await PagedList<T>.CreateAsync(
                EntitySet.OrderByDescending(x => x.CreatedOn),
                paginationFilter.PageNumber,
                paginationFilter.PageSize,
                cancellationToken
            );

        public async Task<PagedList<T>> GetAllPaginatedAsync(
            Expression<Func<T, bool>> predicate,
            PaginationFilter paginationFilter,
            CancellationToken cancellationToken = default
        )
            => await PagedList<T>.CreateAsync(
                EntitySet.Where(predicate).OrderByDescending(x => x.CreatedOn),
                paginationFilter.PageNumber,
                paginationFilter.PageSize,
                cancellationToken
            );

        public async Task<PagedList<T>> GetAllIncludingPaginatedAsync(
            Expression<Func<T, bool>> filter = null,
            PaginationFilter paginationFilter = null,
            CancellationToken cancellationToken = default,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            IQueryable<T> query = EntitySet;
            foreach (var include in includeProperties)
                query = query.Include(include);

            if (filter != null)
                query = query.Where(filter);

            query = query.OrderByDescending(x => x.CreatedOn);

            var pageNumber = paginationFilter?.PageNumber ?? 1;
            var pageSize = paginationFilter?.PageSize ?? int.MaxValue;

            return await PagedList<T>.CreateAsync(
                query,
                pageNumber,
                pageSize,
                cancellationToken
            );
        }

        public async Task<IEnumerable<T>> GetAllIncludingAsync(
            Expression<Func<T, bool>> predicate = null,
            PaginationFilter paginationFilter = null,
            params Expression<Func<T, object>>[] includeProperties
        )
        {
            IQueryable<T> query = EntitySet;
            foreach (var include in includeProperties)
                query = query.Include(include);

            if (predicate != null)
                query = query.Where(predicate);

            query = query.OrderByDescending(x => x.CreatedOn);

            if (paginationFilter == null)
                return await query.ToListAsync();

            return (await PagedList<T>.CreateAsync(
                query,
                paginationFilter.PageNumber,
                paginationFilter.PageSize
            )).Items;
        }

        public void Remove(T entity)
            => _context.Set<T>().Remove(entity);

        public void RemoveRange(IEnumerable<T> entities)
            => _context.Set<T>().RemoveRange(entities);

        public void Update(T entity)
            => _context.Set<T>().Update(entity);

        public void UpdateRange(IEnumerable<T> entities)
            => _context.Set<T>().UpdateRange(entities);
    }

}
