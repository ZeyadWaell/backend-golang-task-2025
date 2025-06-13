using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Domain.Entities;


namespace EasyOrderProduct.Application.Contracts.Interfaces
{
    public interface IInventoryRepository : IGenericRepository<Inventory>
    {
        Task<BaseApiResponse> GetLowStockAsync();
    }
}
