using EasyOrder.Application.Contracts.InterfaceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderAdmin.Application.Queries.Interfaces
{
    public interface IUnitOfWork
    {

        IOrderRepository Orders { get; }

        /// <summary>
        /// Flushes all pending changes to the write database.
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
