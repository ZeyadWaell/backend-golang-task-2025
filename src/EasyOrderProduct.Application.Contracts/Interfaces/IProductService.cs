using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contracts.Interfaces
{
    public interface IProductService
    {
        Task<BaseApiResponse> CreateProductAsync(CreateProductDto dto);
    }
}
