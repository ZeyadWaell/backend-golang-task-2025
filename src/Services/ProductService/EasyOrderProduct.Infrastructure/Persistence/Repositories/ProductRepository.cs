using EasyOrderProduct.Application.Contract.Interfaces.Repository;
using EasyOrderProduct.Application.Contracts.Interfaces;
using EasyOrderProduct.Domain.Entities;
using EasyOrderProduct.Infrastructure.Persistence.Context;
using EasyOrderProduct.Infrastructure.Persistence.Repositories.Main;
using Microsoft.EntityFrameworkCore;


namespace EasyOrderProduct.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(ReadDbContext readContext, WriteDbContext writeContext) : base(readContext, writeContext)
        {
        }

        public async Task<Product> GetWithItemsAndInventoryAsync(int productId)
        {
            return await _readContext.Product
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.Inventory)
                .FirstOrDefaultAsync(p => p.Id == productId);
        }
    }
}
