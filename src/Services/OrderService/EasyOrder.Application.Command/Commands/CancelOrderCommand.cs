using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Command.Commands
{
    public class CancelOrderCommand : IRequest<BaseApiResponse>
    {
        public int OrderId { get; }

        public CancelOrderCommand(int orderId)
        {
            OrderId = orderId;
        }
    }
}
