using AutoMapper; // Ensure this namespace is included
using EasyOrderProduct.Api.Middelware;
using EasyOrderProduct.Application.Command.Commands;
using EasyOrderProduct.Application.Command.Handlers;
using EasyOrderProduct.Application.Queries.Handlers;
using EasyOrderProduct.Infrastructure.Extentions;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Ensure the AutoMapper.Extensions.Microsoft.DependencyInjection package is installed
// via NuGet and the following line is valid
builder.Services
    .AddHttpContextAccessor()
    .AddAutoMapper(typeof(Program).Assembly)
    .AddInfrastructureServices(builder.Configuration) // Ensure this method is implemented in the correct namespace
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithJwt()
    .AddControllers();


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());                          // your API
    cfg.RegisterServicesFromAssembly(typeof(GetProductInventoryQueryHandler).Assembly);         // your Application
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly); // your Infrastructure
});
var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();