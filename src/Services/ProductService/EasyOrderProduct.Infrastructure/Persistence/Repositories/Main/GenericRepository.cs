using EasyOrderProduct.Application.Contracts.Filters;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Domain.Common;
using EasyOrderProduct.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace EasyOrderProduct.Infrastructure.Persistence.Repositories.Main
{
    public class GenericRepository<T> : IGenericRepository<T> where T : Base
    {
        protected readonly ReadDbContext _readContext;
        protected readonly WriteDbContext _writeContext;
        private readonly DbSet<T> _readSet;
        private readonly DbSet<T> _writeSet;

        public GenericRepository(ReadDbContext readContext, WriteDbContext writeContext)
        {
            _readContext = readContext;
            _writeContext = writeContext;
            _readSet = _readContext.Set<T>();
            _writeSet = _writeContext.Set<T>();
        }

        // --- QUERIES (use _readSet) ---

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _readSet.AnyAsync(predicate, ct);

        public T Get(Expression<Func<T, bool>> predicate)
            => _readSet.FirstOrDefault(predicate);

        public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _readSet.FirstOrDefaultAsync(predicate, ct);

        public Task<T> GetIncludingAsync(Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> q = _readSet;
            foreach (var inc in includes) q = q.Include(inc);
            return q.FirstOrDefaultAsync(predicate);
        }

        public IEnumerable<T> GetAll(PaginationFilter filter = null)
        {
            IQueryable<T> q = _readSet.OrderByDescending(x => x.CreatedOn);
            if (filter != null)
                q = q.Skip((filter.PageNumber - 1) * filter.PageSize)
                     .Take(filter.PageSize);
            return q.ToList();
        }

        public IQueryable<T> GetAllQ(PaginationFilter filter = null)
        {
            IQueryable<T> q = _readSet.OrderByDescending(x => x.CreatedOn);
            if (filter != null)
                q = q.Skip((filter.PageNumber - 1) * filter.PageSize)
                     .Take(filter.PageSize);
            return q;
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> predicate, PaginationFilter filter = null)
        {
            IQueryable<T> q = _readSet.Where(predicate).OrderByDescending(x => x.CreatedOn);
            if (filter != null)
                q = q.Skip((filter.PageNumber - 1) * filter.PageSize)
                     .Take(filter.PageSize);
            return q.ToList();
        }

        public async Task<IEnumerable<T>> GetAllAsync(CancellationToken ct = default)
            => await _readSet.OrderByDescending(x => x.CreatedOn).ToListAsync(ct);

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _readSet.Where(predicate)
                             .OrderByDescending(x => x.CreatedOn)
                             .ToListAsync(ct);

        public async Task<IEnumerable<T>> GetAllNoTrackingAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await _readSet.AsNoTracking()
                             .Where(predicate)
                             .OrderByDescending(x => x.CreatedOn)
                             .ToListAsync(ct);

        public Task<PagedList<T>> GetAllPaginatedAsync(PaginationFilter filter, CancellationToken ct = default)
            => PagedList<T>.CreateAsync(
                _readSet.OrderByDescending(x => x.CreatedOn),
                filter?.PageNumber ?? 1,
                filter?.PageSize ?? int.MaxValue,
                ct
            );

        public Task<PagedList<T>> GetAllPaginatedAsync(Expression<Func<T, bool>> predicate,
                                                       PaginationFilter filter,
                                                       CancellationToken ct = default)
            => PagedList<T>.CreateAsync(
                _readSet.Where(predicate).OrderByDescending(x => x.CreatedOn),
                filter?.PageNumber ?? 1,
                filter?.PageSize ?? int.MaxValue,
                ct
            );

        public Task<PagedList<T>> GetAllIncludingPaginatedAsync(
            Expression<Func<T, bool>> filterPredicate = null,
            PaginationFilter filter = null,
            CancellationToken ct = default,
            params Expression<Func<T, object>>[] includes
        )
        {
            IQueryable<T> q = _readSet;
            if (includes?.Length > 0)
                foreach (var inc in includes) q = q.Include(inc);
            if (filterPredicate != null) q = q.Where(filterPredicate);
            q = q.OrderByDescending(x => x.CreatedOn);
            return PagedList<T>.CreateAsync(
                q,
                filter?.PageNumber ?? 1,
                filter?.PageSize ?? int.MaxValue,
                ct
            );
        }

        public async Task<IEnumerable<T>> GetAllIncludingAsync(
            Expression<Func<T, bool>> predicate = null,
            PaginationFilter filter = null,
            params Expression<Func<T, object>>[] includes
        )
        {
            IQueryable<T> q = _readSet;
            if (includes?.Length > 0)
                foreach (var inc in includes) q = q.Include(inc);
            if (predicate != null) q = q.Where(predicate);
            q = q.OrderByDescending(x => x.CreatedOn);
            if (filter == null)
                return await q.ToListAsync();
            var pg = await PagedList<T>.CreateAsync(q, filter.PageNumber, filter.PageSize);
            return pg.Items;
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> filter = null, CancellationToken ct = default)
            => filter == null
               ? _readSet.CountAsync(ct)
               : _readSet.CountAsync(filter, ct);

        // --- COMMANDS (use _writeSet + _writeContext) ---

        public void Add(T entity) => _writeSet.Add(entity);
        public Task AddAsync(T entity, CancellationToken ct = default)
                                      => _writeSet.AddAsync(entity, ct).AsTask();

        public void AddRange(IEnumerable<T> entities)
                                      => _writeSet.AddRange(entities);
        public Task AddRangeAsync(IEnumerable<T> entities, CancellationToken ct = default)
                                      => _writeSet.AddRangeAsync(entities, ct);

        public void Update(T entity) => _writeSet.Update(entity);
        public void UpdateRange(IEnumerable<T> entities)
                                      => _writeSet.UpdateRange(entities);

        public void Remove(T entity) => _writeSet.Remove(entity);
        public void RemoveRange(IEnumerable<T> entities)
                                      => _writeSet.RemoveRange(entities);

        public Task<int> SaveChangesAsync(CancellationToken ct = default)
            => _writeContext.SaveChangesAsync(ct);
    }
}