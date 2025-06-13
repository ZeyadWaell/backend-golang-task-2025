using AutoMapper;
using EasyOrderProduct.Api.Middelware;
using EasyOrderProduct.Application.Command.Commands;
using EasyOrderProduct.Application.Command.Handlers;
using EasyOrderProduct.Application.Queries.Handlers;
using EasyOrderProduct.Infrastructure.Extentions;
using System.Reflection;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services
    .AddHttpContextAccessor()
    .AddAutoMapper(typeof(Program).Assembly)
    .AddInfrastructureServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithJwt()
    .AddControllers();


builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());                         
    cfg.RegisterServicesFromAssembly(typeof(GetProductInventoryQueryHandler).Assembly);         
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly); 
});

var app = builder.Build();


app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyOrder API v1");
    c.RoutePrefix = "swagger";   
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();