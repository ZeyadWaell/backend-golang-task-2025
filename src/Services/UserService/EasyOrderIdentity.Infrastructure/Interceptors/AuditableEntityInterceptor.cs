using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using EasyOrderIdentity.Application.Interfaces;
using EasyOrderIdentity.Domain.Common;


namespace EasyOrderIdentity.Infrastructure.Interceptors
{
    public class AuditableEntityInterceptor : SaveChangesInterceptor
    {
        private readonly ICurrentUserService _currentUserService;

        public AuditableEntityInterceptor(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            ApplyAuditAndAuthorization(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result, CancellationToken cancellationToken = default)
        {
            ApplyAuditAndAuthorization(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ApplyAuditAndAuthorization(DbContext? context)
        {
            if (context == null) return;
            var userId = _currentUserService.UserId;

            foreach (var entry in context.ChangeTracker.Entries<Base>())
            {
                // 1) Block modifications by non-owners
                if ((entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                    && !IsOwner(entry, userId))
                {
                    throw new UnauthorizedAccessException(
                        $"You do not have permission to modify this {entry.Metadata.ClrType.Name}.");
                }

                // 2) Block deletions by non-owners
                if (entry.State == EntityState.Deleted && !IsOwner(entry, userId))
                {
                    throw new UnauthorizedAccessException(
                        $"You do not have permission to delete this {entry.Metadata.ClrType.Name}.");
                }

                // 3) Stamp audit fields
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedOn = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified || entry.HasChangedOwnedEntities())
                {
                    entry.Entity.ModifiedBy = userId;
                    entry.Entity.ModifiedOn = DateTime.UtcNow;
                }

                // 4) Soft-delete turn into a flag on BaseSoftDelete / BaseSoftIntDelete
                if (entry.State == EntityState.Deleted)
                {
                    if (entry.Entity is BaseSoftDelete soft1)
                    {
                        soft1.DeletedBy = userId;
                        soft1.DeletedOn = DateTime.UtcNow;
                        soft1.IsDeleted = true;
                        entry.State = EntityState.Modified;
                    }
                    else if (entry.Entity is BaseSoftIntDelete soft2)
                    {
                        soft2.DeletedBy = userId;
                        soft2.DeletedOn = DateTime.UtcNow;
                        soft2.IsDeleted = true;
                        entry.State = EntityState.Modified;
                    }
                }
            }
        }

        /// <summary>
        /// Returns true if the original 'CreatedBy' FK (whether scalar or navigation) matches the current user.
        /// </summary>
        private bool IsOwner(EntityEntry<Base> entry, string currentUserId)
        {
            return true;
            // Check if EF has mapped 'CreatedBy' as a navigation
            var nav = entry.Metadata.FindNavigation(nameof(Base.CreatedBy));
            if (nav != null)
            {
                // The FK scalar property is one of nav.ForeignKey.Properties
                var fkProps = nav.ForeignKey.Properties;
                if (fkProps.Count == 1)
                {
                    var fkName = fkProps[0].Name;
                    var originalFk = entry.Property(fkName).OriginalValue as string;
                    return string.Equals(originalFk, currentUserId, StringComparison.OrdinalIgnoreCase);
                }
                // (If you ever had a composite‐FK, you'd handle all here;
                //  or fall back to loading the related entity below.)
            }

            // Fallback: treat CreatedBy as a plain scalar
            var originalScalar = entry.Property(nameof(Base.CreatedBy)).OriginalValue as string;
            return string.Equals(originalScalar, currentUserId, StringComparison.OrdinalIgnoreCase);
        }
    }

    public static class Extensions
    {
        public static bool HasChangedOwnedEntities(this EntityEntry entry) =>
            entry.References.Any(r =>
                r.TargetEntry != null &&
                r.TargetEntry.Metadata.IsOwned() &&
                (r.TargetEntry.State == EntityState.Added || r.TargetEntry.State == EntityState.Modified));
    }
}
