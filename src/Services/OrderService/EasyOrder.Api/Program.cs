
using EasyOrder.Api.Middelware;
using EasyOrder.Application.Command.Handlers.Admin;
using EasyOrder.Application.Command.Handlers.Order;
using EasyOrder.Application.Queries.Handlers.Admin;
using EasyOrder.Application.Queries.Handlers.Order;
using EasyOrder.Infrastructure.Extentions;

var builder = WebApplication.CreateBuilder(args);



builder.Services
    .AddHttpContextAccessor()
    .AddAutoMapper(typeof(Program).Assembly)
    .AddInfrastructureServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithJwt()
    .AddControllers();


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
app.Run();
