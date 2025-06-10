using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Queries.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.Queries
{
    public record GetOrderByIdQuery(int Id) : IRequest<BaseApiResponse>;
}