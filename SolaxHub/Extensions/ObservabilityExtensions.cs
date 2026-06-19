using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SolaxHub.Extensions;

internal static class ObservabilityExtensions
{
    public static IServiceCollection AddSolaxHubObservability(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration["OpenTelemetry:ApplicationInsights:ConnectionString"];
        var azureMonitorEnabled = string.IsNullOrWhiteSpace(connectionString) is false;

        services
            .AddOpenTelemetry()
            .ConfigureResource(builder => builder.AddService(nameof(SolaxHub)))
            .WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                if (azureMonitorEnabled)
                {
                    tracing.AddAzureMonitorTraceExporter(options => options.ConnectionString = connectionString);
                }
            })
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation();
                if (azureMonitorEnabled)
                {
                    metrics.AddAzureMonitorMetricExporter(options => options.ConnectionString = connectionString);
                }
            });

        return services;
    }

    public static ILoggingBuilder AddSolaxHubLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        var connectionString = configuration["OpenTelemetry:ApplicationInsights:ConnectionString"];

        builder.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            if (string.IsNullOrWhiteSpace(connectionString) is false)
            {
                options.AddAzureMonitorLogExporter(exporterOptions => exporterOptions.ConnectionString = connectionString);
            }
        });

        return builder;
    }
}
