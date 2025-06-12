using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.Interfaces;
using EasyOrderProduct.Application.Queries.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Queries.Handlers
{
    public class GetProductInventoryQueryHandler : IRequestHandler<GetProductInventoryQuery, BaseApiResponse>
    {
        private readonly IProductService _productService;

        public GetProductInventoryQueryHandler(IProductService productService)
        {
            _productService = productService;
        }

        public Task<BaseApiResponse> Handle(GetProductInventoryQuery request, CancellationToken cancellationToken)
        {
            return _productService.GetInventoryAsync(request.Id);
        }
    }
}
