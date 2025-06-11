using EasyOrderProduct.Application.Command.Commands;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using EasyOrderProduct.Application.Contracts.Interfaces;
using EasyOrderProduct.Application.Contracts.Services;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Command.Handlers
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, BaseApiResponse>
    {
        private readonly IProductService _productService;
        public CreateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<BaseApiResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            return await _productService.CreateProductAsync(request.ProductDto);
        }
    }
}
