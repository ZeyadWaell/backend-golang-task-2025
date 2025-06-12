using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using MediatR;


namespace EasyOrder.Application.Queries.Queries.Order
{
    public record GetOrderStatusQuery(int id) : IRequest<BaseApiResponse>;
}
