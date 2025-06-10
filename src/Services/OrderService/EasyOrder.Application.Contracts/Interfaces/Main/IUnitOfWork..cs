using EasyOrder.Application.Contracts.Interfaces.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Interfaces.Main
{
    public interface IUnitOfWork
    {

        IOrderRepository OrdersRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
