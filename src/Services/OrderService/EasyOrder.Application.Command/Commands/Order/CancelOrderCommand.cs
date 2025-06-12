using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using MediatR;


namespace EasyOrder.Application.Command.Commands.Order
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
