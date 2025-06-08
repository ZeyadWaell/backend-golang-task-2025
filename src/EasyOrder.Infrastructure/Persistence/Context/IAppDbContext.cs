using EasyOrder.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.Persistence.Context
{
    public interface IAppDbContext
    {
        /// <summary>
        /// Exposes a DbSet for any aggregate/root entity
        /// </summary>
        DbSet<TEntity> Set<TEntity>() where TEntity : Base;

        /// <summary>
        /// Save transactionally all pending changes
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// (Optional) access to EF’s Database facade for transactions, migrations, etc.
        /// </summary>
        DatabaseFacade Database { get; }
    }
}
