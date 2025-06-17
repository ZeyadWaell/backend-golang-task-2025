﻿using EasyOrderProduct.Application.Contract.Interfaces.Services;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Queries.Queries;
using MediatR;


namespace EasyOrderProduct.Application.Queries.Handlers
{
    public class GetAllLowStockQueryHandler : IRequestHandler<GetAllLowStockQuery, BaseApiResponse>
    {
        private readonly IAdminService _adminService;

        public GetAllLowStockQueryHandler(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<BaseApiResponse> Handle(GetAllLowStockQuery request, CancellationToken cancellationToken)
        {
           return await _adminService.GetAllLowStockAsync();
        }
    }
}
