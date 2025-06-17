using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Contract.Interfaces.Services
{
    public interface IAdminService
    {
        Task<BaseApiResponse> GetAllLowStockAsync();
    }
}
