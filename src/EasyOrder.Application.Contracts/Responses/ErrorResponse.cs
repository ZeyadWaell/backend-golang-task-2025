using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Application.Contracts.Responses
{
    public class ErrorResponse : BaseApiResponse
    {
        public string? Details { get; set; }

        public ErrorResponse(string message, int statusCode, string? details = null)
            : base(false, message, statusCode)
        {
            Details = details;
        }

        public static ErrorResponse BadRequest(string message = "Invalid request.", string? details = null)
        {
            return new ErrorResponse(message, 400, details);
        }

        public static ErrorResponse Unauthorized(string message = "Unauthorized access.")
        {
            return new ErrorResponse(message, 401);
        }

        public static ErrorResponse NotFound(string message = "Resource not found.")
        {
            return new ErrorResponse(message, 404);
        }

        public static ErrorResponse InternalServerError(string message = "An unexpected error occurred.", string? details = null)
        {
            return new ErrorResponse(message, 500, details);
        }
    }
}
