using SolaxHub.Application.Dashboard.Options;
using SolaxHub.Dashboard;
using SolaxHub.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSolaxHub(builder.Configuration)
    .AddSolaxHubObservability(builder.Configuration);

builder.Logging.AddSolaxHubLogging(builder.Configuration);

var dashboardEnabled = builder.Configuration.GetSection(nameof(DashboardOptions)).Get<DashboardOptions>()?.Enabled is true;
var dashboardPort = builder.Configuration.GetSection(nameof(DashboardOptions)).Get<DashboardOptions>()?.Port ?? 8080;

// Bind Kestrel to the dashboard port when enabled; otherwise bind no endpoints so the host
// behaves like the original worker service (no listening port).
builder.WebHost.UseUrls(dashboardEnabled ? $"http://*:{dashboardPort}" : string.Empty);

var app = builder.Build();

if (dashboardEnabled)
{
    app.MapDashboard();
}

await app.RunAsync();
