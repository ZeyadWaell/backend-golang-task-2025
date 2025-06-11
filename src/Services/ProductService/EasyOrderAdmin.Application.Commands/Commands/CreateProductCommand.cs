using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderProduct.Application.Command.Commands
{
    public class CreateProductCommand : IRequest<BaseApiResponse>
    {
        public CreateProductDto ProductDto { get; }
        public CreateProductCommand(CreateProductDto productDto)
        {
            ProductDto = productDto;
        }
    }
}
