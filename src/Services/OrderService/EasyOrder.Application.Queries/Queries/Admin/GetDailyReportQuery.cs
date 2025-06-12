using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.Queries.Admin
{
    public record GetDailyReportQuery(DateTime time) : IRequest<BaseApiResponse>;
}
