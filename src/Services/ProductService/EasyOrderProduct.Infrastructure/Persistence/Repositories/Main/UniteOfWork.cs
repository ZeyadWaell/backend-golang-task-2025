using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Infrastructure.Persistence.Context;
using EasyOrderProduct.Infrastructure.Persistence.Repositories;
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
        #region private
        private readonly ReadDbContext _readContext;
        private readonly WriteDbContext _writeContext;
        private bool _disposed;
        #endregion

        #region public
        public IProductRepository ProductRepository { get; }
        public IInventoryRepository InventoryRepository { get; }

        public IProductItemRepository ProductItemRepository { get; }
        #endregion

        public UnitOfWork(ReadDbContext readContext, WriteDbContext writeContext)
        {
            _readContext = readContext;
            _writeContext = writeContext;
            ProductRepository = new ProductRepository(_readContext, _writeContext);
            InventoryRepository = new InventoryRepository(_readContext, _writeContext);
            ProductItemRepository = new ProductItemRepository(_readContext, _writeContext);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _writeContext.SaveChangesAsync(cancellationToken);
        }

        #region Dispose
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
        #endregion
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
