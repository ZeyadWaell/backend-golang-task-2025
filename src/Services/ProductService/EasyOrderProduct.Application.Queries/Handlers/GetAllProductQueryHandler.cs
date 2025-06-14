using EasyOrderProduct.Application.Contract.Interfaces.Services;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Queries.Queries;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Queries.Handlers
{
    public class GetAllProductQueryHandler : IRequestHandler<GetAllProductQuery, BaseApiResponse>
    {
        private readonly IProductService _productService;

        public GetAllProductQueryHandler(IProductService productService)
        {
            _productService = productService;
        }


        public Task<BaseApiResponse> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
        {
            return _productService.GetAllAsync(request.filter);
        }
    }
}
