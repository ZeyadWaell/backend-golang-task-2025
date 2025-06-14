using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.Interfaces.Services
{
    public interface IProductService
    {
        Task<BaseApiResponse> CreateProductAsync(UpsertProductDto dto);
        Task<BaseApiResponse> GetAllAsync(PaginationFilter filter);
        Task<BaseApiResponse> GetByIdAsync(int id);
        Task<BaseApiResponse> UpsertAsync(UpsertProductDto dto);
        Task<BaseApiResponse> GetInventoryAsync(int productId);
    }
}
