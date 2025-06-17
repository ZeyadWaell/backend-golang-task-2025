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
        public async Task<bool> TryReserveAsync(int productItemId, int qty)
        {
            var affected = await _readContext.inventories
                .Where(i => i.ProductItemId == productItemId && i.QuantityOnHand >= qty)
                .ExecuteUpdateAsync(b => b
                    .SetProperty(i => i.QuantityOnHand, i => i.QuantityOnHand - qty));
            return affected > 0;
        }

        public async Task<bool> IncrementAsync(int productItemId, int qty)
        {
            var exists = await _readContext.inventories
                .AnyAsync(i => i.ProductItemId == productItemId);

            if (!exists)
                return false;

            var updatedRows = await _readContext.inventories
                .Where(i => i.ProductItemId == productItemId && i.QuantityOnHand >= qty)
                .ExecuteUpdateAsync(b => b
                    .SetProperty(
                        i => i.QuantityOnHand,
                        i => i.QuantityOnHand + qty
                    )
                );

            return updatedRows > 0;
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
