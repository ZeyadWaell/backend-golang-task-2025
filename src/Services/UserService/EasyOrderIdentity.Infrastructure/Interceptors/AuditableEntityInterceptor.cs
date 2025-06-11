// Infrastructure/Interceptors/AuditableEntityInterceptor.cs
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyOrderIdentity.Application.Interfaces;
using EasyOrderIdentity.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EasyOrderIdentity.Infrastructure.Interceptors
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUser;

        public AuditableEntityInterceptor(ICurrentUserService currentUser)
        {
            _currentUser = currentUser
                ?? throw new ArgumentNullException(nameof(currentUser));
        }

        private void ApplyAuditAndAuthorization(DbContext ctx)
        {
            var now = DateTime.UtcNow;
            var userId = _currentUser.UserId;

            var entries = ctx.ChangeTracker
                             .Entries<BaseEntity>()
                             .Where(e => e.State == EntityState.Added
                                      || e.State == EntityState.Modified
                                      || e.State == EntityState.Deleted);

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = now;
                        entry.Entity.CreatedBy = userId;
                        break;

                    case EntityState.Modified:
                        var originalCreator = entry.OriginalValues
                                                   .GetValue<string>("CreatedBy");
                        if (!string.Equals(originalCreator, userId, StringComparison.Ordinal))
                            throw new UnauthorizedAccessException(
                                "You cannot modify entities you did not create.");

                        entry.Entity.ModifiedOn = now;
                        entry.Entity.ModifiedBy = userId;
                        break;

                    case EntityState.Deleted:
                        var creator = entry.OriginalValues
                                           .GetValue<string>("CreatedBy");
                        if (!string.Equals(creator, userId, StringComparison.Ordinal))
                            throw new UnauthorizedAccessException(
                                "You cannot delete entities you did not create.");
                        break;
                }
            }
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,InterceptionResult<int> result)
        {
            if (eventData.Context != null)
                ApplyAuditAndAuthorization(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
                ApplyAuditAndAuthorization(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
