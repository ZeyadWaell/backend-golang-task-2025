using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(s["Key"]))
                    };
                    o.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = ctx =>
                        {
                            var h = ctx.Request.Headers["Authorization"].ToString();
                            if (string.IsNullOrEmpty(h) || !h.StartsWith("Bearer "))
                                throw new UnauthorizedAccessException("No Bearer token provided");
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = ctx =>
                            throw new UnauthorizedAccessException($"Token validation failed: {ctx.Exception.Message}"),
                        OnChallenge = ctx =>
                        {
                            ctx.HandleResponse();
                            throw new UnauthorizedAccessException("You are not authenticated");
                        },
                        OnForbidden = ctx =>
                            throw new UnauthorizedAccessException("You do not have permission"),
                    };
                });
            return services;
        }
    }
}
