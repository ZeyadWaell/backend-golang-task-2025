using EasyOrder.Infrastructure.Extentions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using System.Text.Json;

namespace EasyOrder.Infrastructure.Extentions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwaggerWithJwt(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EasyOrderIdentity API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                    {
                        new OpenApiSecurityScheme {
                            Reference = new OpenApiReference {
                                Type = ReferenceType.SecurityScheme,
                                Id   = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });
            return services;
        }
        public static WebApplication UseSwaggerWithUI(this WebApplication app)
        {
            app.UseSwagger();

            // now serve at /swagger
            SwaggerUIBuilderExtensions.UseSwaggerUI(app, c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyOrder API V1");
                c.RoutePrefix = "swagger";
            });
            app.MapGet("/swagger/v1/swagger.json", async context =>
            {
                var provider = context.RequestServices.GetRequiredService<ISwaggerProvider>();
                var swaggerDoc = provider.GetSwagger("v1");
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(swaggerDoc, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });
            })
           .AllowAnonymous();
            return app;
        }
    }
}
