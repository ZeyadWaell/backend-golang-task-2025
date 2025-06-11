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
    public class UpdateProductCommand : IRequest<BaseApiResponse>
    {
        public UpsertProductDto ProductDto { get; }
        public UpdateProductCommand(UpsertProductDto productDto)
        {
            ProductDto = productDto;
        }
    }
}
