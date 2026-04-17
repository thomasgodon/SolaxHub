using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace SolaxHub.Extensions;

internal static class ObservabilityExtensions
{
    public static IServiceCollection AddSolaxHubObservability(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOpenTelemetry()
            .ConfigureResource(builder => builder.AddService(nameof(SolaxHub)))
            .WithTracing(tracing => tracing
                .AddAspNetCoreInstrumentation()
                .AddAzureMonitorTraceExporter(options =>
                {
                    options.ConnectionString = configuration["OpenTelemetry:ApplicationInsights:ConnectionString"];
                }))
            .WithMetrics(metrics => metrics
                .AddAzureMonitorMetricExporter(options =>
                {
                    options.ConnectionString = configuration["OpenTelemetry:ApplicationInsights:ConnectionString"];
                })
                .AddAspNetCoreInstrumentation());

        return services;
    }

    public static ILoggingBuilder AddSolaxHubLogging(this ILoggingBuilder builder, IConfiguration configuration)
    {
        builder.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.AddAzureMonitorLogExporter(exporterOptions =>
            {
                exporterOptions.ConnectionString = configuration["OpenTelemetry:ApplicationInsights:ConnectionString"];
            });
        });

        return builder;
    }
}
