using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Domain.Entities;
using EasyOrderProduct.Infrastructure.Persistence.Context;
using EasyOrderProduct.Infrastructure.Persistence.Repositories.Main;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Infrastructure.Persistence.Repositories
{
    public class ProductItemRepository : GenericRepository<ProductItem>, IProductItemRepository
    {
        public ProductItemRepository(ReadDbContext readContext, WriteDbContext writeContext) : base(readContext, writeContext)
        {
        }

        public async Task<bool> CheckAvailabilityAsync(int productItemId)
        {
            return await _readContext.ProductItem
                .Include(pi => pi.Inventory)
                .Where(pi => pi.Id == productItemId && pi.Inventory.QuantityOnHand >= 1)
                .AnyAsync();
        }
    }
}
