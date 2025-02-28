using SolaxHub.IotHub.Extensions;
using SolaxHub.IotHub.Models;
using SolaxHub.Knx.Extensions;
using SolaxHub.Solax.Extensions;
using SolaxHub.Udp.Extensions;

namespace SolaxHub.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSolaxHub(this IServiceCollection serviceCollection, IConfiguration configuration)
        => serviceCollection
            .Configure<IotHubOptions>(configuration.GetSection(nameof(IotHubOptions)))
            .AddSolaxClients(configuration)
            .AddUdpSender(configuration)
            .AddIotHub(configuration)
            .AddKnx(configuration)
            .AddMediatR(m => m.RegisterServicesFromAssembly(typeof(Program).Assembly));
}