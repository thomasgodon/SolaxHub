using SolaxHub.IotHub.Extensions;
using SolaxHub.Knx.Extensions;
using SolaxHub.Solax.Extensions;
using SolaxHub.Solax.Workers;
using SolaxHub.Udp.Extensions;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var configuration = hostContext.Configuration;
        services.AddHostedService<SolaxModbusWorker>();
        services.AddSolaxClients(configuration);
        services.AddUdpSender(configuration);
        services.AddIotCentral(configuration);
        services.AddKnx(configuration);
        services.AddMediatR(m => m.RegisterServicesFromAssembly(typeof(Program).Assembly));
    })
    .Build();

await host.RunAsync();
