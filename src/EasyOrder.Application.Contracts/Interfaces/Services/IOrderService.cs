using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Contracts.Responses.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Interfaces.Services
{
    public interface IOrderService
    {
        Task<BaseApiResponse> GetAllOrderAsync(PaginationFilter paginationFilter);
        Task<BaseApiResponse> GetOrderByIdAsync(int id);
        Task<BaseApiResponse> CreateOrderAsync(CreateOrderDto dto); // Ensure CreateOrderCommand is defined in the correct namespace
        Task<BaseApiResponse> CancelOrderAsync(int id);
        Task<BaseApiResponse> GetOrderStatus(int id);
    }
}
