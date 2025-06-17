using EasyOrderProduct.Application.Contract.Interfaces.Repository;

namespace EasyOrderProduct.Application.Contracts.Interfaces.Main
{
    public interface IUnitOfWork
    {

        IInventoryRepository InventoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductItemRepository ProductItemRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
