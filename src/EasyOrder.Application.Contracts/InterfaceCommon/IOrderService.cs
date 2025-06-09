using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Contracts.Responses.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.InterfaceCommon
{
    public interface IOrderService
    {
        Task<BaseApiResponse> GetAllOrderAsync(PaginationFilter paginationFilter);
        Task<BaseApiResponse> GetOrderByIdAsync(int id);
    }
}
