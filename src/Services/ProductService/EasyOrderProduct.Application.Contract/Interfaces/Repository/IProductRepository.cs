using EasyOrderProduct.Application.Contract.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.Interfaces.Repository
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<ProductDetailsResponseDto> GetProductDetailsDtoAsync(int productId);
        Task<Product> GetWithItemsAndInventoryAsync(int productId);
    }
}
