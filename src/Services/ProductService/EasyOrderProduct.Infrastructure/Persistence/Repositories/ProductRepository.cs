using AutoMapper;
using EasyOrderProduct.Application.Contract.DTOs.Responses;
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
        private readonly IMapper _mapper;

        public ProductRepository(ReadDbContext readContext, WriteDbContext writeContext) : base(readContext, writeContext)
        {
        }

        public ProductRepository(ReadDbContext readContext, WriteDbContext writeContext, IMapper mapper) : base(readContext, writeContext)
        {
            _mapper = mapper;
        }
        public async Task<ProductDetailsResponseDto> GetProductDetailsDtoAsync(int productId)
        {
            // Eager-load the full object graph
            var product = await _readContext.Set<Product>()
                .Where(p => p.Id == productId)
                .Include(p => p.Variations)
                    .ThenInclude(v => v.Options)
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.Options)
                        .ThenInclude(pio => pio.VariationOption)
                .Include(p => p.ProductItems)
                    .ThenInclude(pi => pi.Inventory)
                .FirstOrDefaultAsync();

            if (product == null)
                return null; 

            // Map to your DTO
            return _mapper.Map<ProductDetailsResponseDto>(product);
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
