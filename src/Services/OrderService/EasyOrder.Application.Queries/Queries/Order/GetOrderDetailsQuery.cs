using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using MediatR;

namespace EasyOrder.Application.Queries.Queries.Order
{
    public record GetOrderByIdQuery(int Id) : IRequest<BaseApiResponse>;
}