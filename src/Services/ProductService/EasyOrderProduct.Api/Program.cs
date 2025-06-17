using EasyOrderProduct.Api.Middelware;
using EasyOrderProduct.Application.Command.Handlers;
using EasyOrderProduct.Application.Contract.Hubs;
using EasyOrderProduct.Application.Contract.Interfaces.GrpsServices;
using EasyOrderProduct.Application.Contracts.Mapping;
using EasyOrderProduct.Application.Queries.Handlers;
using EasyOrderProduct.Infrastructure.Extentions;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Ocelot.Values;

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

builder.Services.AddSignalR();

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
        lo.Protocols = HttpProtocols.Http1AndHttp2;
        lo.UseHttps();
    });
});

var app = builder.Build();

//app//.UseMiddleware<ExceptionHandlingMiddleware>();

// 1) expose swagger.json
app.UseSwagger();

// 2) expose swagger UI
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "EasyOrder API v1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<InventoryHub>("/hubs/inventory");

app.MapGrpcService<InventoryCheckerService>();
app.MapGet("/", () => "Inventory gRPC at https://localhost:7003");

app.Run();