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
            .AddSolaxHub(configuration);
    })
    .Build();

await host.RunAsync();
