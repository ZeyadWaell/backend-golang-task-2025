using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Queries.Queries.Order;
using MediatR;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.Handlers.Order
{
    public class GetOrderStatusQueryHandler : IRequestHandler<GetOrderStatusQuery, BaseApiResponse>
    {
        private readonly IOrderService _orderService;
        public GetOrderStatusQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseApiResponse> Handle(GetOrderStatusQuery request, CancellationToken cancellationToken)
        {
            var result = await _orderService.GetOrderStatus(request.id);

            return result;
        }
    }
}
