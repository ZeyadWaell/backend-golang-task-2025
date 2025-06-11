using EasyOrderIdentity.Api.Extensions;
using EasyOrderIdentity.Api.Middleware;
using EasyOrderIdentity.Infrastructure;
using EasyOrderIdentity.Infrastructure.Extentions;
using EasyOrderIdentity.Infrastructure.ProgramServices;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddSwaggerWithJwt()
    .AddControllers();

var app = builder.Build();

app.SeedIdentity();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
