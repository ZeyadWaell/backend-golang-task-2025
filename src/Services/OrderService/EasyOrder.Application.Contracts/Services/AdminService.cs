using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AdminService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BaseApiResponse> GetAllOrdersAsync(PaginationFilter filter)
        {

            var order = await _unitOfWork.OrdersRepository.GetAllPaginatedAsync(filter);
            return new SuccessResponse<object>("Order Fetched", order, 200);

        }
        public async Task<BaseApiResponse> UpdateOrderStatus(int id, OrderStatusDto dto)
        {
            var order = await _unitOfWork.OrdersRepository.GetAsync(x => x.Id == id);
            if (order == null)
                return ErrorResponse.NotFound("Order not found");

            order.Status = dto.orderStatus;
            _unitOfWork.OrdersRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return new SuccessResponse<object>("Order status updated successfully", null, 200);
        }
        public async Task<BaseApiResponse> GetDailyReportAsync(DateTime date)
        {
            var report = await _unitOfWork.OrdersRepository.GetAsync(x => x.CreatedOn == date);
            if (report == null)
                return ErrorResponse.NotFound("No report found for the specified date");

            return new SuccessResponse<object>("Daily report fetched successfully", report, 200);
        }
    }
}
