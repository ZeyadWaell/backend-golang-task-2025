using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<Product> GetWithItemsAndInventoryAsync(int productId);
    }
}
