using EasyOrderProduct.Api.Middelware;
using EasyOrderProduct.Application.Command.Handlers;
using EasyOrderProduct.Application.Contract.Interfaces.GrpsServices;
using EasyOrderProduct.Application.Contracts.Mapping;
using EasyOrderProduct.Application.Queries.Handlers;
using EasyOrderProduct.Infrastructure.Extentions;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services
    .AddHttpContextAccessor()
    .AddAutoMapper(typeof(ProductProfile).Assembly)
    .AddInfrastructureServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithJwt()
    .AddControllers();

builder.Services.AddGrpc();

//builder.Services.AddSingleton(provider =>
//{
//    var channel = GrpcChannel.ForAddress("https://localhost:7003");
//    return new InventoryService.InventoryServiceClient(channel);
//});

//builder.Services.AddScoped<IInventoryCheckerService, GrpcInventoryChecker>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<CreateProductCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<UpdateProductCommandHandler>();

    cfg.RegisterServicesFromAssemblyContaining<GetAllProductQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetProductByIdQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetProductInventoryQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetAllLowStockQueryHandler>();
});

builder.WebHost.ConfigureKestrel(opts =>
{
    opts.ListenLocalhost(7003, lo =>
    {
        lo.Protocols = HttpProtocols.Http2;
        lo.UseHttps();
    });
});

builder.Services.AddGrpc(o =>
{
    o.EnableDetailedErrors = true;
});

var app = builder.Build();


app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyOrder API v1");
    c.RoutePrefix = "swagger";   
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapGrpcService<InventoryCheckerService>();
app.MapGet("/", () => "Inventory gRPC at https://localhost:7003"); app.UseSwagger();

app.Run();