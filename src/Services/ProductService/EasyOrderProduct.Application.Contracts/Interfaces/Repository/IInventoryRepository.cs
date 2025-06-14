using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Domain.Entities;


namespace EasyOrderProduct.Application.Contract.Interfaces.Repository
{
    public interface IInventoryRepository : IGenericRepository<Inventory>
    {
        Task<BaseApiResponse> GetLowStockAsync();
        Task<bool> TryReserveAsync(int productItemId, int qty);
        Task IncrementAsync(int productItemId, int qty);
    }
}
