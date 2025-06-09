using EasyOrder.Api.Routes;
using EasyOrder.Application.Contracts.Filters;
using EasyOrder.Application.Queries.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        /// <summary>
        /// Retrieves a paginated list of all orders for the current user.
        /// </summary>
        /// <param name="paginationFilter">
        ///   Paging parameters (PageNumber, PageSize) to control the slice of data returned.
        /// </param>
        /// <returns>
        ///   A <see cref="BaseApiResponse"/> containing the paged list of orders and the appropriate HTTP status code.
        /// </returns>
        [HttpGet(OrderRoutes.GetAll)]
        public async Task<IActionResult> GetAllOrders([FromQuery] PaginationFilter paginationFilter)
        {
            var query = new GetAllOrdersQuery(paginationFilter);

            var response = await _mediator.Send(query);

            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Retrieves the details of a single order by its unique identifier.
        /// </summary>
        /// <param name="id">The database ID of the order to retrieve.</param>
        /// <returns>
        ///   A <see cref="BaseApiResponse"/> containing the order details (if found) or an error message and status code.
        /// </returns>
        [HttpGet(OrderRoutes.GetById)]
        public async Task<IActionResult> GetOrderById(int id)
        {
            var query = new GetOrderByIdQuery(id);

            var response = await _mediator.Send(query);

            return StatusCode(response.StatusCode, response);
        }

    }
}
