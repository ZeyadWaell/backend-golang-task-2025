using AutoMapper;
using EasyOrder.Application.Command.Commands;
using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.Interfaces.InternalServices;
using EasyOrder.Application.Contracts.Interfaces.Main;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Contracts.Responses.Global;
using EasyOrder.Domain.Entities;
using MediatR;

namespace EasyOrder.Application.Command.Handlers
{
    public class CreateOrderCommandHandler: IRequestHandler<CreateOrderCommand, BaseApiResponse>
    {
        private readonly IOrderService _orderService;

        public CreateOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseApiResponse> Handle(CreateOrderCommand request,CancellationToken ct)
        {
            var result = await _orderService.CreateOrderAsync(request.OrderDto);
            return new SuccessResponse<object>(
                "Order created successfully",
                null,
                201
            );
        }
    }
}
