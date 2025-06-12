using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using System.Threading.Tasks;

namespace EasyOrder.Infrastructure.Extentions
{
    public static class JwtExtensions
    {
        public static IServiceCollection AddJwtAuthentication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var s = configuration.GetSection("Jwt");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(o =>
                {
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = s["Issuer"],
                        ValidAudience = s["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(s["Key"]))
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            // *** Skip JWT for Swagger UI & JSON docs ***
                            var path = ctx.HttpContext.Request.Path;
                            if (path.StartsWithSegments("/swagger"))
                            {
                                ctx.NoResult();
                                return Task.CompletedTask;
                            }

                            // original logic
                            var h = ctx.Request.Headers["Authorization"].ToString();
                            if (string.IsNullOrEmpty(h) || !h.StartsWith("Bearer "))
                                throw new UnauthorizedAccessException("No Bearer token provided");
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = ctx =>
                        {
                            // let it flow as a 401, not an exception
                            ctx.NoResult();
                            ctx.Response.StatusCode = 401;
                            ctx.Response.ContentType = "application/json";
                            return ctx.Response.WriteAsync(
                                $"{{\"error\":\"Token validation failed: {ctx.Exception.Message}\"}}");
                        },
                        OnChallenge = ctx =>
                        {
                            ctx.HandleResponse();
                            ctx.Response.StatusCode = 401;
                            return ctx.Response.WriteAsync(
                                "{\"error\":\"You are not authenticated\"}");
                        },
                        OnForbidden = ctx =>
                        {
                            ctx.Response.StatusCode = 403;
                            return ctx.Response.WriteAsync(
                                "{\"error\":\"You do not have permission\"}");
                        }
                    };
                });

            return services;
        }
    }
}
