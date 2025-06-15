using EasyOrderProduct.Application.Command.Commands;
using EasyOrderProduct.Application.Contract.Interfaces.Services;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using MediatR;


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
