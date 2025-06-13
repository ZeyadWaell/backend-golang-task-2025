using System.Text.Json;

namespace OcelotApiGateWay
{
    public class SwaggerDebugMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SwaggerDebugMiddleware> _logger;

        public SwaggerDebugMiddleware(RequestDelegate next, ILogger<SwaggerDebugMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext ctx)
        {
            // only intercept the swagger docs endpoints
            if (!ctx.Request.Path.StartsWithSegments("/swagger/docs"))
            {
                await _next(ctx);
                return;
            }

            try
            {
                await _next(ctx);
            }
            catch (Exception ex)
            {
                // log full details server-side
                _logger.LogError(ex, "Unhandled exception while proxying {Path}", ctx.Request.Path);

                // return full details client-side (DEV only!)
                ctx.Response.StatusCode = 500;
                ctx.Response.ContentType = "application/json";

                var errorPayload = new
                {
                    ex.Message,
                    ex.StackTrace,
                    Inner = ex.InnerException?.ToString()
                };

                await ctx.Response.WriteAsync(JsonSerializer.Serialize(errorPayload));
            }
        }
    }
}