using Hangfire;
using Hangfire.SqlServer;
using Microsoft.EntityFrameworkCore;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using MMLib.SwaggerForOcelot.DependencyInjection;
using MMLib.Ocelot.Provider.AppConfiguration;
using OcelotApiGateWay.Middelware;
using OcelotApiGateWay.Context;
using OcelotApiGateWay.Tasks;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddOcelotWithSwaggerSupport(options => options.Folder = "OcelotConfiguration");
builder.Services
    .AddOcelot(builder.Configuration)
    .AddAppConfiguration();
builder.Services.AddSwaggerForOcelot(builder.Configuration);

var hangfireCs = builder.Configuration.GetConnectionString("HangfireConnection");
if (string.IsNullOrWhiteSpace(hangfireCs))
    throw new InvalidOperationException("Missing HangfireConnection in ConnectionStrings!");
builder.Services
    .AddHangfire(cfg => cfg
        .UseSqlServerStorage(
            hangfireCs,
            new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                DisableGlobalLocks = true
            }))
    .AddHangfireServer();

builder.Services.AddDbContext<HangFireContext>(options =>
    options.UseSqlServer(hangfireCs));

// 4) Background tasks
builder.Services.AddTransient<AuditLogJob>();

// 5) MVC, Razor, SignalR
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Hangfire dashboard
app.UseHangfireDashboard("/hangfire");

// SwaggerForOcelot UI
app.UseSwaggerForOcelotUI(opt => {
    opt.PathToSwaggerGenerator = "/swagger/docs";
});

// Exception middleware
app.UseWhen(
    ctx => !ctx.Request.Headers.ContainsKey("X-Skip-ExceptionHandling"),
    branch => branch.UseMiddleware<ExceptionHandlingMiddleware>()
);

// Endpoints
app.UseEndpoints(endpoints => {
    endpoints.MapControllers();
    endpoints.MapRazorPages();
});

// Ocelot last
await app.UseOcelot();

app.Run();