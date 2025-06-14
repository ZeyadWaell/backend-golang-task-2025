using EasyOrderProduct.Application.Contracts.Interfaces.Main;
using EasyOrderProduct.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.Interfaces.Repository
{
    public interface IProductItemRepository : IGenericRepository<ProductItem>
    {
        Task<bool> CheckAvailabilityAsync(int productItemId);
    }
}
