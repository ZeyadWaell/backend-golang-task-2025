using EasyOrder.Api.Routes;
using EasyOrder.Application.Command.Commands.Admin;
using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Queries.Handlers.Admin;
using EasyOrder.Application.Queries.Queries.Admin;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EasyOrder.Api.Controllers
{
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdminController(IMediator mediator = null)
        {
            _mediator = mediator;
        }
        [HttpGet]
        [Route(AdminOrderRoutes.GetAll)]
        public async Task<IActionResult> GetAllOrder(PaginationFilter filter)
        {
            var query = new GetAllOrderAdminQuery(filter);
            var response = await _mediator.Send(query);
            return StatusCode(response.StatusCode, response);
        }
        [HttpPut]
        [Route(AdminOrderRoutes.Update)]
        public async Task<IActionResult> UpdateOrder([FromRoute] int id,OrderStatusDto request)
        {
            var command = new UpdateOrderStatusCommand(id, request);
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet]
        [Route(AdminOrderRoutes.DailyReport)]
        public async Task<IActionResult> GetDailyReports([FromQuery] DateTime date)
        {
            var command = new GetDailyReportQuery(date);
            var response = await _mediator.Send(command);
            return StatusCode(response.StatusCode, response);
        }
    }
}
