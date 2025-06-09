using AutoMapper;
using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Contracts.InterfaceCommon;
using EasyOrder.Application.Contracts.Responses.Global;
using EasyOrder.Application.Queries.DTOs;
using EasyOrderAdmin.Application.Queries.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public OrderService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<BaseApiResponse> GetAllOrderAsync(PaginationFilter paginationFilter)
        {
            var pagedProperties = await _unitOfWork.OrdersRepository.GetAllPaginatedAsync(x => x.CreatedBy == _currentUserService.UserId, paginationFilter);

            return new SuccessResponse<object>("Got All Orders", pagedProperties, 200);
        }
        public async Task<BaseApiResponse> GetOrderByIdAsync(int id)
        {
            var order = await _unitOfWork.OrdersRepository.GetIncludingAsync(x => x.Id == id, x => x.Items, x => x.Payment);

            if (order == null)
                return ErrorResponse.NotFound("Order not found");

            var dto = _mapper.Map<OrderDetailsDto>(order);

            return new SuccessResponse<object>("Got Order", dto, 200);
        }
    } 
}
