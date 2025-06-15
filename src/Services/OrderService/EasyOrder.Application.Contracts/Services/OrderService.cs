using AutoMapper;
using EasyOrder.Application.Contracts.BackGroundService;
using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Contracts.Interfaces.GrpcServices;
using EasyOrder.Application.Contracts.Interfaces.InternalServices;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Contracts.Messaging;
using EasyOrder.Domain.Entities;
using EasyOrder.Domain.Enums;
using EasyOrderProduct.Application.Contracts.Protos;
using Hangfire;
using MassTransit;
using Ocelot.Infrastructure;


namespace EasyOrder.Application.Queries.Services
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IInventoryChecker _inventoryChecker;
        private readonly IBus _bus;
        private readonly IBackgroundJobClient _jobs;

        public OrderService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper, IInventoryChecker inventoryChecker = null, IBus bus = null, IBackgroundJobClient jobs = null)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
            _inventoryChecker = inventoryChecker;
            _bus = bus;
            _jobs = jobs;
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


            //foreach (var item in dto.Items) old method 
            //{
            //    var isAvailable = await _inventoryChecker.CheckAvailabilityAsync(item.ProductItemId);
            //    if (!isAvailable)
            //        return ErrorResponse.BadRequest($"ProductItem {item.ProductItemId} is out of stock");
            //}

            var reservedItems = new List<(int ProductItemId, int Qty)>();
            foreach (var item in dto.Items)
            {
                var reserved = await _inventoryChecker.ReserveAsync(item.ProductItemId, item.Quantity);
                if (!reserved)
                {
                    foreach (var (pid, qty) in reservedItems)
                        await _inventoryChecker.IncrementAsync(pid, qty);

                    return ErrorResponse.BadRequest($"Failed to reserve inventory for ProductItem {item.ProductItemId}");
                }
                reservedItems.Add((item.ProductItemId, item.Quantity));
            }

            var order = _mapper.Map<Order>(dto);
            await _unitOfWork.OrdersRepository.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();

            _jobs.Enqueue<ChargePaymentJob>(job => job.ExecuteAsync(dto.Payment.Method, order.TotalAmount,order.Id));

            return new SuccessResponse<object>(
                "Order created successfully",
                order.Id,
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
