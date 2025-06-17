using EasyOrder.Api.Routes;
using EasyOrder.Application.Contracts.DTOs;
using EasyOrder.Application.Queries.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Command.Commands;
using EasyOrder.Application.Queries.Queries.Order;
using EasyOrder.Application.Command.Commands.Order;
using Microsoft.AspNetCore.RateLimiting;

namespace EasyOrder.Api.Controllers
{
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(OrderRoutes.GetAll)]
        public async Task<IActionResult> GetAllOrders([FromQuery] PaginationFilter paginationFilter)
        {
            var query = new GetAllOrdersQuery(paginationFilter);
            var response = await _mediator.Send(query);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet(OrderRoutes.GetById)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var query = new GetOrderByIdQuery(id);
            var response = await _mediator.Send(query);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost(OrderRoutes.Create)]
      //  [EnableRateLimiting("FixedPolicy")]      

        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto dto)
        {
            var query = new CreateOrderCommand(dto);
            var response = await _mediator.Send(query);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut(OrderRoutes.Update)]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var query = new CancelOrderCommand(id);
            var response = await _mediator.Send(query);
            return StatusCode(response.StatusCode, response);
        }
        [HttpGet(OrderRoutes.GetStatus)]
        public async Task<IActionResult> GetOrderStatus(int id)
        {
            var query = new GetOrderStatusQuery(id);
            var response = await _mediator.Send(query);
            return StatusCode(response.StatusCode, response);
        }
    }
}
