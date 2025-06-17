using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using OcelotApiGateWay.Responses.Global;
using OcelotApiGateWay.Tasks;

namespace OcelotApiGateWay.Middelware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        private readonly IBackgroundJobClient _jobs;

        public ExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<ExceptionHandlingMiddleware> logger,
            IBackgroundJobClient jobs)
        {
            _next = next;
            _logger = logger;
            _jobs = jobs;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestBody = await ReadRequestBodyAsync(context.Request);

            var originalBody = context.Response.Body;
            await using var buffer = new MemoryStream();
            context.Response.Body = buffer;

            // initialize variables to avoid unassigned use
            int statusCode = StatusCodes.Status200OK;
            string responseBody = string.Empty;

            try
            {
                await _next(context);
                statusCode = context.Response.StatusCode;
            }
            catch (UnauthorizedAccessException uaEx)
            {
                statusCode = StatusCodes.Status401Unauthorized;
                await WriteErrorResponseAsync(context, statusCode, ErrorResponse.Unauthorized(uaEx.Message));
            }
            catch (Exception ex)
            {
                statusCode = StatusCodes.Status500InternalServerError;
                await WriteErrorResponseAsync(context, statusCode, ErrorResponse.InternalServerError(ex.Message));
            }
            finally
            {
                buffer.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(buffer, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
                responseBody = await reader.ReadToEndAsync();

                buffer.Seek(0, SeekOrigin.Begin);
                await buffer.CopyToAsync(originalBody);
                context.Response.Body = originalBody;

                stopwatch.Stop();
                _logger.LogInformation(
                    "HTTP {Method} {Path} responded {StatusCode} in {Elapsed}ms. RequestBody: {ReqBody}",
                    context.Request.Method,
                    context.Request.Path,
                    statusCode,
                    stopwatch.ElapsedMilliseconds,
                    requestBody);

                // Enqueue audit log job
                _jobs.Enqueue<AuditLogJob>(job => job.SaveAsync(
                    context.Request.Path,
                    requestBody,
                    responseBody,
                    statusCode));
            }
        }
        

        private static Task WriteErrorResponseAsync(HttpContext context, int statusCode, ErrorResponse error)
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
