using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Repository;
using EasyOrder.Infrastructure.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.Persistence.Repositories.Main
{
    /// <summary>
    /// Concrete Unit‐of‐Work for the Order microservice.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ReadDbContext _readContext;
        private readonly WriteDbContext _writeContext;
        private bool _disposed;

        public IOrderRepository OrdersRepository { get; }

        public UnitOfWork(ReadDbContext readContext, WriteDbContext writeContext)
        {
            _readContext = readContext;
            _writeContext = writeContext;
            OrdersRepository = new OrderRepository(_readContext, _writeContext);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _writeContext.SaveChangesAsync(cancellationToken);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _readContext?.Dispose();
                    _writeContext?.Dispose();
                }
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
