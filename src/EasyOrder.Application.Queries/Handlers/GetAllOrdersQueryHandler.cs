using EasyOrder.Application.Contracts.InterfaceCommon;
using EasyOrder.Application.Contracts.Responses.Global;
using EasyOrder.Application.Queries.DTOs;
using EasyOrder.Application.Queries.Queries;
using EasyOrderAdmin.Application.Queries.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.Handlers
{
    public class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, BaseApiResponse>
    {
        private readonly IOrderService _orderService;

        public GetAllOrdersQueryHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseApiResponse> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderService.GetAllOrderAsync(request.filter);

            return orders;
        }
    }
}
