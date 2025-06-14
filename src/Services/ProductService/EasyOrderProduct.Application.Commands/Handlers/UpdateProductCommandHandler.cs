using EasyOrderProduct.Application.Command.Commands;
using EasyOrderProduct.Application.Contract.Interfaces.Services;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Command.Handlers
{
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, BaseApiResponse>
    {
        private readonly IProductService _productService;

        public UpdateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }
        public Task<BaseApiResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            return _productService.UpsertAsync(request.ProductDto);
        }
    }
}
