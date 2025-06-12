using Azure.Monitor.OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using SolaxHub.Extensions;
using SolaxHub.Knx.Workers;
using SolaxHub.Solax.Workers;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services
            .AddHostedService<SolaxModbusWorker>()
            .AddHostedService<KnxConnectionWorker>()
            .AddHostedService<KnxReceiverWorker>()
            .AddSolaxHub(configuration)
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
    })
    .ConfigureLogging((context, builder) =>
    {
        builder.AddOpenTelemetry(options =>
        {
            options.IncludeScopes = true;
            options.ParseStateValues = true;
            options.AddAzureMonitorLogExporter(exporterOptions =>
            {
                exporterOptions.ConnectionString = context.Configuration["OpenTelemetry:ApplicationInsights:ConnectionString"];
            });
        });
    })
    .Build();

await host.RunAsync();
