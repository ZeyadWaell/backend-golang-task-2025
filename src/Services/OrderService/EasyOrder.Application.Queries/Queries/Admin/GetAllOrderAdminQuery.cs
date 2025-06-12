using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Filters;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.Queries.Admin
{
    public record GetAllOrderAdminQuery(PaginationFilter filter) : IRequest<BaseApiResponse>;
}
