using EasyOrder.Application.Contracts.Responses.Global;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.Queries
{
    public record GetOrderStatusQuery(int id) : IRequest<BaseApiResponse>;
}
