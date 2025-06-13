using EasyOrderProduct.Api.Routes;
using EasyOrderProduct.Application.Command.Commands;
using EasyOrderProduct.Application.Contracts.DTOs.Responses;
using EasyOrderProduct.Application.Contracts.Filters;
using EasyOrderProduct.Application.Contracts.Interfaces;
using EasyOrderProduct.Application.Queries.Queries;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NpgsqlTypes;

namespace EasyOrderProduct.Api.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Route(ProductRoutes.Create)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> CreateProduct(UpsertProductDto request)
        {
            var product = new CreateProductCommand(request);
            var result = await _mediator.Send(product);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Route(ProductRoutes.GetById)]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var product = new GetProductByIdQuery(id);
            var result = await _mediator.Send(product);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Route(ProductRoutes.GetAll)]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] PaginationFilter filter)
        {
            var products = new GetAllProductQuery(filter);
            var result = await _mediator.Send(products);
            return StatusCode(result.StatusCode, result);
        }

        [HttpPut]
        [Route(ProductRoutes.Update)]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(int id, UpsertProductDto request)
        {
            request.Id = id;
            var product = new UpdateProductCommand(request);
            var result = await _mediator.Send(product);
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet]
        [Route(ProductRoutes.CheckInventory)]
        [AllowAnonymous]
        public async Task<IActionResult> CheckInventory([FromRoute]int id)
        {
            var product = new GetProductInventoryQuery(id);
            var result = await _mediator.Send(product);
            return StatusCode(result.StatusCode, result);
        }
    }
}
