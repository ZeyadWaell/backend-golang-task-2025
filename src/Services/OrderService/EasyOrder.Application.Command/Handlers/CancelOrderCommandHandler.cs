using EasyOrder.Application.Command.Commands;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;

using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Domain.Enums;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Command.Handlers
{
    public class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand, BaseApiResponse>
    {
        private readonly IOrderService _orderService;

        public CancelOrderCommandHandler(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<BaseApiResponse> Handle(CancelOrderCommand request, CancellationToken ct)
        {
            var order = await _orderService.CancelOrderAsync(request.OrderId);
            return new SuccessResponse<object>(
                "Order cancelled successfully",
                null,
                200
            );
        }
    }
}