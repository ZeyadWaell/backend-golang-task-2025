using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Filters;
using MediatR;

namespace EasyOrder.Application.Queries.Queries.Order
{
    public record GetAllOrdersQuery(PaginationFilter filter): IRequest<BaseApiResponse>;
}
