using SolaxHub.IotHub.Extensions;
using SolaxHub.Knx.Extensions;
using SolaxHub.Knx.Workers;
using SolaxHub.Solax.Extensions;
using SolaxHub.Solax.Workers;
using SolaxHub.Udp.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services
            .AddHostedService<SolaxModbusWorker>()
            .AddHostedService<KnxReceiverWorker>()
            .AddSolaxClients(configuration)
            .AddUdpSender(configuration)
            .AddIotCentral(configuration)
            .AddKnx(configuration)
            .AddMediatR(m => m.RegisterServicesFromAssembly(typeof(Program).Assembly));
    })
    .Build();

await host.RunAsync();
