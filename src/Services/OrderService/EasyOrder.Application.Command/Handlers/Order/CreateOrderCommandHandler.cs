using EasyOrder.Application.Command.Commands.Order;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Interfaces.Services;
using MediatR;

namespace EasyOrder.Application.Command.Handlers.Order
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
