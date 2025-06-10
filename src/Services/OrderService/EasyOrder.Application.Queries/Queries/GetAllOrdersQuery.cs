using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Contracts.Responses.Global;
using EasyOrder.Application.Queries.DTOs;
using EasyOrder.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.Queries
{
    public record GetAllOrdersQuery(PaginationFilter filter): IRequest<BaseApiResponse>;
}
