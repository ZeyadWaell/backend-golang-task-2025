using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrderIdentity.Api.Extensions
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var jwtSection = configuration.GetSection("Jwt");
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];
            var key = jwtSection["Key"];

            if (string.IsNullOrWhiteSpace(issuer)
             || string.IsNullOrWhiteSpace(audience)
             || string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("JWT settings are missing or invalid in configuration.");
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = signingKey
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var http = context.HttpContext;

                            // 1) If this endpoint allows anonymous, skip JWT
                            var endpoint = http.GetEndpoint();
                            if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
                            {
                                context.NoResult();
                                return Task.CompletedTask;
                            }

                            // 2) Skip Swagger UI & JSON
                            var path = http.Request.Path;
                            if (path.StartsWithSegments("/swagger"))
                            {
                                context.NoResult();
                                return Task.CompletedTask;
                            }

                            // 3) Otherwise enforce Bearer token
                            var header = http.Request.Headers["Authorization"].ToString();
                            if (string.IsNullOrWhiteSpace(header) ||
                                !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                context.NoResult();
                                http.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                return http.Response.WriteAsync(
                                    "{\"error\":\"No Bearerss token provided\"}");
                            }

                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = context =>
                        {
                            context.NoResult();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync(
                                $"{{\"error\":\"Token validation failed: {context.Exception.Message}\"}}");
                        },

                        OnChallenge = context =>
                        {
                            context.HandleResponse();
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync(
                                "{\"error\":\"You are not authenticated\"}");
                        },

                        OnForbidden = context =>
                        {
                            context.Response.StatusCode = StatusCodes.Status403Forbidden;
                            context.Response.ContentType = "application/json";
                            return context.Response.WriteAsync(
                                "{\"error\":\"You do not have permission\"}");
                        }
                    };
                });

            return services;
        }
    }
}
