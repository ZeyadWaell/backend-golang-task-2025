using EasyOrderProduct.Application.Contract.DTOs.Responses;
using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Domain.Entities;
using EasyOrderProduct.Infrastructure.Persistence.Context;
using EasyOrderProduct.Infrastructure.Persistence.Repositories.Main;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Infrastructure.Persistence.Repositories
{
    public class InventoryRepository : GenericRepository<Inventory>, IInventoryRepository
    {
        public InventoryRepository(ReadDbContext readContext, WriteDbContext writeContext) : base(readContext, writeContext)
        {
        }

        public async Task<BaseApiResponse> GetLowStockAsync()
        {
            var result = await _readContext.ProductItem
            .Include(pi => pi.Inventory)
            .Where(pi => pi.Inventory.QuantityOnHand < 10)
            .Select(pi => new LowStockItemDto
            {
                ProductItemId = pi.Id,
                Sku = pi.Sku,
                QuantityOnHand = pi.Inventory.QuantityOnHand,
                WarehouseLocation = pi.Inventory.WarehouseLocation
            }).ToListAsync();

            return new SuccessResponse<object>("returned low stock items", result);
        }
    }
}
