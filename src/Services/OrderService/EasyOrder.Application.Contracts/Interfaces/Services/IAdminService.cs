using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Interfaces.Services
{
    public interface IAdminService
    {
        Task<BaseApiResponse> GetAllOrdersAsync(PaginationFilter filter);
        Task<BaseApiResponse> UpdateOrderStatus(int id, OrderStatusDto dto);
        Task<BaseApiResponse> GetDailyReportAsync(DateTime date);
    }
}
