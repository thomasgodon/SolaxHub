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
                    .AddAspNetCoreInstrumentation());
            
    })
    .Build();

await host.RunAsync();
