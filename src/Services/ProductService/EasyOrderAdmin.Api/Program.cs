using AutoMapper; // Ensure this namespace is included
using EasyOrderProduct.Infrastructure.Extentions;
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

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();