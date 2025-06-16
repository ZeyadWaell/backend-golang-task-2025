
using EasyOrder.Api.Middelware;
using EasyOrder.Application.Command.Handlers.Admin;
using EasyOrder.Application.Command.Handlers.Order;
using EasyOrder.Application.Contracts.Hubs;
using EasyOrder.Application.Queries.Handlers.Admin;
using EasyOrder.Application.Queries.Handlers.Order;
using EasyOrder.Application.Queries.Mappings;
using EasyOrder.Infrastructure.ActionFilter;
using EasyOrder.Infrastructure.Extentions;
using Hangfire;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);



builder.Services
    .AddHttpContextAccessor()
    .AddAutoMapper(typeof(OrderMappingProfile).Assembly)
    .AddInfrastructureServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithJwt()
    .AddControllers();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter(
        policyName: "FixedPolicy",
        options =>
        {
            options.PermitLimit = 10;
            options.Window = TimeSpan.FromMinutes(1);
            options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
            options.QueueLimit = 0;
        });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

builder.Services.AddMediatR(cfg =>
{
    // Command handlers (Order)
    cfg.RegisterServicesFromAssemblyContaining<CreateOrderCommandHandler>();
    cfg.RegisterServicesFromAssemblyContaining<CancelOrderCommandHandler>();

    // Command handlers (Admin)
    cfg.RegisterServicesFromAssemblyContaining<UpdateOrderStatusCommandHandler>();

    // Query handlers (Order)
    cfg.RegisterServicesFromAssemblyContaining<GetAllOrdersQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetOrderByIdQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetOrderStatusQueryHandler>();

    // Query handlers (Admin)
    cfg.RegisterServicesFromAssemblyContaining<GetAllOrderAdminQueryHandler>();
    cfg.RegisterServicesFromAssemblyContaining<GetDailyReportQueryHandler>();
});


var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<OrderStatusHub>("/hubs/orderStatus");
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { new AllowAllDashboardAuthorizationFilter() }
});
app.Run();
