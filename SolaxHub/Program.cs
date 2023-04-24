using SolaxHub;
using SolaxHub.IotCentral.Extensions;
using SolaxHub.Knx.Extensions;
using SolaxHub.Solax.Extensions;
using SolaxHub.Udp.Extensions;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddSolaxClients(configuration);
        services.AddUdpSender(configuration);
        services.AddIotCentral(configuration);
        services.AddKnx(configuration);
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();
