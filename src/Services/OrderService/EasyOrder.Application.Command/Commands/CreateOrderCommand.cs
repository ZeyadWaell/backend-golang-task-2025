using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using MediatR;


namespace EasyOrder.Application.Command.Commands
{
    public class CreateOrderCommand : IRequest<BaseApiResponse>
    {
        public CreateOrderDto OrderDto { get; }

        public CreateOrderCommand(CreateOrderDto orderDto)
        {
            OrderDto = orderDto;
        }
    }
}
