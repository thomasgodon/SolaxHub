using SolaxHub;
using SolaxHub.IotHub.Extensions;
using SolaxHub.Knx.Extensions;
using SolaxHub.Solax.Extensions;
using SolaxHub.Udp.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddHostedService<Worker>();
        services.AddSolaxClients(configuration);
        services.AddUdpSender(configuration);
        services.AddIotCentral(configuration);
        services.AddKnx(configuration);
    })
    .Build();

await host.RunAsync();
