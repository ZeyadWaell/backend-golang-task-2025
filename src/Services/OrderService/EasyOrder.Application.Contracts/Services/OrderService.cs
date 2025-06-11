using AutoMapper;
using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Contracts.Interfaces.InternalServices;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Domain.Entities;
using EasyOrder.Domain.Enums;


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

        public async Task<BaseApiResponse> CreateOrderAsync(CreateOrderDto dto)
        {
            if (dto == null)
                return ErrorResponse.BadRequest("Order payload cannot be null");

            if (dto.Items == null || !dto.Items.Any())
                return ErrorResponse.BadRequest("You must include at least one order item");

            if (dto.Items.Any(i => i.Quantity <= 0))
                return ErrorResponse.BadRequest("Each item quantity must be at least 1");

            var order = _mapper.Map<Order>(dto);

            await _unitOfWork.OrdersRepository.AddAsync(order);
            var saved = await _unitOfWork.SaveChangesAsync();

            var resultDto = _mapper.Map<OrderDetailsDto>(order);
            return new SuccessResponse<object>(
                message: "Order created successfully",
                data: resultDto,
                 200
            );
        }

        public async Task<BaseApiResponse> CancelOrderAsync(int id)
        {
            var order = await _unitOfWork.OrdersRepository.GetAsync(x => x.Id == id && x.CreatedBy == _currentUserService.UserId);
            if (order == null)
                return ErrorResponse.NotFound("Order not found");

            if (order.Status != OrderStatus.Pending)
                return ErrorResponse.BadRequest("Only pending orders can be cancelled");

            order.Status = OrderStatus.Cancelled;
            _unitOfWork.OrdersRepository.Update(order);
            await _unitOfWork.SaveChangesAsync();
            return new SuccessResponse<object>("Order cancelled successfully", null, 200);

        }
        public async Task<BaseApiResponse> GetOrderStatus(int id)
        {
            var order = await _unitOfWork.OrdersRepository.GetAsync(x => x.Id == id && x.CreatedBy == _currentUserService.UserId);
            if (order == null)
                return ErrorResponse.NotFound("Order not found");

            var resultDto = _mapper.Map<OrderStatusDto>(order);

            return new SuccessResponse<object>("Order status retrieved successfully", resultDto, 200);
        }
    }
}
