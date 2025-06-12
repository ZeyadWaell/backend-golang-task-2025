

using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Queries.Queries.Admin;
using MediatR;

namespace EasyOrder.Application.Queries.Handlers.Admin
{
    public class GetDailyReportQueryHandler : IRequestHandler<GetDailyReportQuery, BaseApiResponse>
    {
        private readonly IAdminService _adminService;
        public GetDailyReportQueryHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }
        async Task<BaseApiResponse> IRequestHandler<GetDailyReportQuery, BaseApiResponse>.Handle(GetDailyReportQuery request, CancellationToken cancellationToken)
        {
            return await _adminService.GetDailyReportAsync(request.time);
        }
    }
}
