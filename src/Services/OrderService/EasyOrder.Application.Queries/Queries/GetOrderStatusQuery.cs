using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using MediatR;


namespace EasyOrder.Application.Queries.Queries
{
    public record GetOrderStatusQuery(int id) : IRequest<BaseApiResponse>;
}
