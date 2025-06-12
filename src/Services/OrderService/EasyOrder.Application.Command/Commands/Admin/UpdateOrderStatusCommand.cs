using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Command.Commands.Admin
{
   public  class UpdateOrderStatusCommand : IRequest<BaseApiResponse>
    {
        public int Id { get; }
        public OrderStatusDto Status { get; }
        public UpdateOrderStatusCommand(int orderId, OrderStatusDto status)
        {
            Id = orderId;
            Status = status;
        }
    }

}
