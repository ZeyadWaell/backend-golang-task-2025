using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using MediatR;

namespace EasyOrder.Application.Queries.Queries
{
    public record GetOrderByIdQuery(int Id) : IRequest<BaseApiResponse>;
}