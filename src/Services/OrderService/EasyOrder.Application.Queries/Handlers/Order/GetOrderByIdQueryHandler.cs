using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Queries.Queries.Order;
using MediatR;

namespace EasyOrder.Application.Queries.Handlers.Order
{
    public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, BaseApiResponse>
    {
        private readonly IOrderService _orderService;

        public GetOrderByIdQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseApiResponse> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
        {
            var order = await _orderService.GetOrderByIdAsync(request.Id);
            return order;
        }
    }
}
