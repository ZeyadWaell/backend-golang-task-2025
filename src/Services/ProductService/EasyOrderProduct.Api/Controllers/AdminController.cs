using EasyOrderProduct.Api.Routes;
using EasyOrderProduct.Application.Queries.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NpgsqlTypes;

namespace EasyOrderProduct.Api.Controllers
{
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route(AdminRoutes.GetAllLowStock)]
        public async Task<IActionResult> GetAllLowStock()
        {
            var query = new GetAllLowStockQuery();
            var result = await _mediator.Send(query);
            return StatusCode(result.StatusCode, result);
        }
    }
}
