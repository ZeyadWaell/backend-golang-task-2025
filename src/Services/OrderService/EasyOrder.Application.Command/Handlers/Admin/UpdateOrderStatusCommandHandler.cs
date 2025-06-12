using EasyOrder.Application.Command.Commands.Admin;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using EasyOrder.Application.Contracts.Interfaces.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Command.Handlers.Admin
{
    public class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand, BaseApiResponse>
    {
        private readonly IAdminService _adminService;
        public UpdateOrderStatusCommandHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }
        public async Task<BaseApiResponse> Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
           return await _adminService.UpdateOrderStatus(request.Id, request.Status);
        }
    }
}
