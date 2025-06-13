using EasyOrderProduct.Application.Contracts.DTOs.Responses.Global;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EasyOrderProduct.Infrastructure.Extentions
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
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = signingKey
                    };

                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            var http = ctx.HttpContext;
                            var path = http.Request.Path;

                            // 1) Skip JWT for any [AllowAnonymous] endpoints
                            var endpoint = http.GetEndpoint();
                            if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() != null
                                || path.StartsWithSegments("/swagger"))
                            {
                                ctx.NoResult();
                                return Task.CompletedTask;
                            }

                            // 2) Enforce Bearer header
                            var header = http.Request.Headers["Authorization"].ToString();
                            if (string.IsNullOrWhiteSpace(header) ||
                                !header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
                            {
                                ctx.NoResult();
                                ctx.Fail("No Bearer token provided");
                            }

                            return Task.CompletedTask;
                        },

                        OnAuthenticationFailed = ctx =>
                        {
                            // invalid token
                            ctx.NoResult();
                            ctx.Fail(ctx.Exception);
                            return Task.CompletedTask;
                        },

                        OnChallenge = async ctx =>
                        {
                            // single responsibility: write one JSON error
                            ctx.HandleResponse();
                            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            ctx.Response.ContentType = "application/json";

                            var error = ctx.Error;
                            var message =
                                error == "No Bearer token provided"
                                ? "No Bearer token provided"
                                : "You are not authenticated";

                            var resp = ErrorResponse.Unauthorized(message);
                            await ctx.Response.WriteAsync(
                                JsonSerializer.Serialize(resp));
                        },

                        OnForbidden = async ctx =>
                        {
                            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
                            ctx.Response.ContentType = "application/json";
                            var resp = ErrorResponse.Forbidden("You do not have permission");
                            await ctx.Response.WriteAsync(
                                JsonSerializer.Serialize(resp));
                        }
                    };
                });

            return services;
        }
    }
}
