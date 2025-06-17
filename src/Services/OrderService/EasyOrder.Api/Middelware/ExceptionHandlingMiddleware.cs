using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using EasyOrder.Application.Contracts.DTOs.Responses.Global;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace EasyOrder.Api.Middelware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestBody = await ReadRequestBodyAsync(context.Request);

            var originalBody = context.Response.Body;
            await using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            try
            {
                await _next(context);

                if (context.Response.StatusCode == StatusCodes.Status429TooManyRequests)
                {
                    _logger.LogWarning("Rate limit exceeded on {Path}", context.Request.Path);

                    buffer.SetLength(0);

                    await WriteErrorResponseAsync(
                        context,
                        StatusCodes.Status429TooManyRequests,
                        ErrorResponse.TooManyRequests("Too many requests. Please try again later."));
                }
            }
            catch (UnauthorizedAccessException uaEx)
            {
                _logger.LogWarning(uaEx, "Unauthorized access to {Path}", context.Request.Path);
                await WriteErrorResponseAsync(
                    context,
                    StatusCodes.Status401Unauthorized,
                    ErrorResponse.Unauthorized(uaEx.Message));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Path}", context.Request.Path);
                await WriteErrorResponseAsync(
                    context,
                    StatusCodes.Status500InternalServerError,
                    ErrorResponse.InternalServerError(details: ex.Message));
            }
            finally
            {
                // rewind and copy the buffer (either original or your custom payload) to the real response stream
                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBody);
                context.Response.Body = originalBody;

                stopwatch.Stop();
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms. RequestBody: {ReqBody}",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds,
                    requestBody);
            }
        }

        private static Task WriteErrorResponseAsync(
            HttpContext context,
            int statusCode,
            ErrorResponse error)
        {
            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(error);
            return context.Response.WriteAsync(payload);
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            if (request.ContentLength == null || request.ContentLength == 0)
                return string.Empty;

            request.EnableBuffering();
            request.Body.Seek(0, SeekOrigin.Begin);

            using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            request.Body.Seek(0, SeekOrigin.Begin);
            return body;
        }
    }
}
