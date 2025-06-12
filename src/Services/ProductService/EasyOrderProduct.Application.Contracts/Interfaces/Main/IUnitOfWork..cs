using EasyOrderProduct.Application.Contracts.Interfaces;

namespace EasyOrderProduct.Application.Contracts.Interfaces.Main
{
    public interface IUnitOfWork
    {

        IInventoryRepository InventoryRepository { get; }
        IProductRepository ProductRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
