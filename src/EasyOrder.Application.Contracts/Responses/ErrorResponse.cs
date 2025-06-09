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


        // 4xx Client Errors
        public static ErrorResponse BadRequest(string message = "Invalid request.", string? details = null)
            => new ErrorResponse(message, 400, details);

        public static ErrorResponse Unauthorized(string message = "Unauthorized access.", string? details = null)
            => new ErrorResponse(message, 401, details);

        public static ErrorResponse PaymentRequired(string message = "Payment required.", string? details = null)
            => new ErrorResponse(message, 402, details);

        public static ErrorResponse Forbidden(string message = "Forbidden.", string? details = null)
            => new ErrorResponse(message, 403, details);

        public static ErrorResponse NotFound(string message = "Resource not found.", string? details = null)
            => new ErrorResponse(message, 404, details);

        public static ErrorResponse MethodNotAllowed(string message = "Method not allowed.", string? details = null)
            => new ErrorResponse(message, 405, details);

        public static ErrorResponse NotAcceptable(string message = "Not acceptable.", string? details = null)
            => new ErrorResponse(message, 406, details);

        public static ErrorResponse ProxyAuthenticationRequired(string message = "Proxy authentication required.", string? details = null)
            => new ErrorResponse(message, 407, details);

        public static ErrorResponse RequestTimeout(string message = "Request timeout.", string? details = null)
            => new ErrorResponse(message, 408, details);

        public static ErrorResponse Conflict(string message = "Conflict occurred.", string? details = null)
            => new ErrorResponse(message, 409, details);

        public static ErrorResponse Gone(string message = "Resource gone.", string? details = null)
            => new ErrorResponse(message, 410, details);

        public static ErrorResponse LengthRequired(string message = "Length required.", string? details = null)
            => new ErrorResponse(message, 411, details);

        public static ErrorResponse PreconditionFailed(string message = "Precondition failed.", string? details = null)
            => new ErrorResponse(message, 412, details);

        public static ErrorResponse PayloadTooLarge(string message = "Payload too large.", string? details = null)
            => new ErrorResponse(message, 413, details);

        public static ErrorResponse URITooLong(string message = "URI too long.", string? details = null)
            => new ErrorResponse(message, 414, details);

        public static ErrorResponse UnsupportedMediaType(string message = "Unsupported media type.", string? details = null)
            => new ErrorResponse(message, 415, details);

        public static ErrorResponse RangeNotSatisfiable(string message = "Range not satisfiable.", string? details = null)
            => new ErrorResponse(message, 416, details);

        public static ErrorResponse ExpectationFailed(string message = "Expectation failed.", string? details = null)
            => new ErrorResponse(message, 417, details);

        public static ErrorResponse ImATeapot(string message = "I'm a teapot.", string? details = null)
            => new ErrorResponse(message, 418, details);

        public static ErrorResponse MisdirectedRequest(string message = "Misdirected request.", string? details = null)
            => new ErrorResponse(message, 421, details);

        public static ErrorResponse UnprocessableEntity(string message = "Unprocessable entity.", string? details = null)
            => new ErrorResponse(message, 422, details);

        public static ErrorResponse Locked(string message = "Resource locked.", string? details = null)
            => new ErrorResponse(message, 423, details);

        public static ErrorResponse FailedDependency(string message = "Failed dependency.", string? details = null)
            => new ErrorResponse(message, 424, details);

        public static ErrorResponse TooEarly(string message = "Too early.", string? details = null)
            => new ErrorResponse(message, 425, details);

        public static ErrorResponse UpgradeRequired(string message = "Upgrade required.", string? details = null)
            => new ErrorResponse(message, 426, details);

        public static ErrorResponse PreconditionRequired(string message = "Precondition required.", string? details = null)
            => new ErrorResponse(message, 428, details);

        public static ErrorResponse TooManyRequests(string message = "Too many requests.", string? details = null)
            => new ErrorResponse(message, 429, details);

        public static ErrorResponse RequestHeaderFieldsTooLarge(string message = "Request header fields too large.", string? details = null)
            => new ErrorResponse(message, 431, details);

        public static ErrorResponse UnavailableForLegalReasons(string message = "Unavailable for legal reasons.", string? details = null)
            => new ErrorResponse(message, 451, details);

        // 5xx Server Errors
        public static ErrorResponse InternalServerError(string message = "An unexpected error occurred.", string? details = null)
            => new ErrorResponse(message, 500, details);

        public static ErrorResponse NotImplemented(string message = "Not implemented.", string? details = null)
            => new ErrorResponse(message, 501, details);

        public static ErrorResponse BadGateway(string message = "Bad gateway.", string? details = null)
            => new ErrorResponse(message, 502, details);

        public static ErrorResponse ServiceUnavailable(string message = "Service unavailable.", string? details = null)
            => new ErrorResponse(message, 503, details);

        public static ErrorResponse GatewayTimeout(string message = "Gateway timeout.", string? details = null)
            => new ErrorResponse(message, 504, details);

        public static ErrorResponse HTTPVersionNotSupported(string message = "HTTP version not supported.", string? details = null)
            => new ErrorResponse(message, 505, details);

        public static ErrorResponse VariantAlsoNegotiates(string message = "Variant also negotiates.", string? details = null)
            => new ErrorResponse(message, 506, details);

        public static ErrorResponse InsufficientStorage(string message = "Insufficient storage.", string? details = null)
            => new ErrorResponse(message, 507, details);

        public static ErrorResponse LoopDetected(string message = "Loop detected.", string? details = null)
            => new ErrorResponse(message, 508, details);

        public static ErrorResponse NotExtended(string message = "Not extended.", string? details = null)
            => new ErrorResponse(message, 510, details);

        public static ErrorResponse NetworkAuthenticationRequired(string message = "Network authentication required.", string? details = null)
            => new ErrorResponse(message, 511, details);
    }
}
