using EasyOrderIdentity.Application.Interfaces;
using EasyOrderIdentity.Domain.Common;
using EasyOrderIdentity.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Infrastructure.Services.Internal
{
    public class OwnershipService : IOwnershipService
    {
        private readonly AppDbContext _db;
        private readonly ICurrentUserService _currentUserService;

        public OwnershipService(AppDbContext db, ICurrentUserService currentUserService)
        {
            _db = db;
            _currentUserService = currentUserService;
        }

        public async Task EnsureOwnerAsync<TEntity>(Guid id) where TEntity : BaseEntity
        {
            var entity = await _db.Set<TEntity>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                throw new KeyNotFoundException($"{typeof(TEntity).Name} with Id '{id}' not found.");

            var userId = _currentUserService.UserId
                         ?? throw new UnauthorizedAccessException("User is not authenticated.");

            if (!string.Equals(entity.CreatedBy, userId, StringComparison.Ordinal))
                throw new UnauthorizedAccessException("Access denied: not the owner of this resource.");
        }
    }
}
