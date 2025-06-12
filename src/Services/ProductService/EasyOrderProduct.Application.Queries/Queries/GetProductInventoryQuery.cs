using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Queries.Queries
{
    public record GetProductInventoryQuery(int Id) : IRequest<BaseApiResponse>;
}
