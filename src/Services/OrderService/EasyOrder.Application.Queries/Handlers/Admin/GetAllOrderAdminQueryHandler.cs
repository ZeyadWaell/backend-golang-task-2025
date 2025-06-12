using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Interfaces.Services;
using EasyOrder.Application.Queries.Queries.Admin;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Queries.Handlers.Admin
{
    public class GetAllOrderAdminQueryHandler : IRequestHandler<GetAllOrderAdminQuery, BaseApiResponse>
    {
        private readonly IAdminService _adminService;
        public GetAllOrderAdminQueryHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public async Task<BaseApiResponse> Handle(GetAllOrderAdminQuery request, CancellationToken cancellationToken)
        {
           return await _adminService.GetAllOrdersAsync(request.filter);
        }
    }
}
