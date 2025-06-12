namespace OcelotApiGateWay
{
    public class SwaggerDebugMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;
        private readonly ILogger<SwaggerDebugMiddleware> _logger;

        public SwaggerDebugMiddleware(
            RequestDelegate next,
            IConfiguration config,
            ILogger<SwaggerDebugMiddleware> logger)
        {
            _next = next;
            _config = config;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only log when hitting Swagger UI or docs endpoint
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                // Read the "SwaggerEndPoints" section from configuration
                var endpointsSection = _config.GetSection("SwaggerEndPoints");
                var swaggerEndpoints = endpointsSection.Get<List<Dictionary<string, string>>>();

                if (swaggerEndpoints == null)
                {
                    _logger.LogError("⚠️ SwaggerEndPoints section is NULL. File not loaded or key missing.");
                }
                else if (!swaggerEndpoints.Any())
                {
                    _logger.LogWarning("⚠️ SwaggerEndPoints section is present but empty.");
                }
                else
                {
                    foreach (var ep in swaggerEndpoints)
                    {
                        // Expect keys like "Key" and "Url"
                        ep.TryGetValue("Key", out var key);
                        ep.TryGetValue("Url", out var url);

                        _logger.LogInformation("🔍 Loaded Swagger endpoint: {Key} -> {Url}",
                            key ?? "(no Key)", url ?? "(no Url)");
                    }
                }
            }

            await _next(context);
        }
    }
}
