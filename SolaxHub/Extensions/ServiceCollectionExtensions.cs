using SolaxHub.Knx.Extensions;
using SolaxHub.Solax.Extensions;
using SolaxHub.Udp.Extensions;

namespace SolaxHub.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSolaxHub(this IServiceCollection serviceCollection, IConfiguration configuration)
        => serviceCollection
            .AddSolaxClients(configuration)
            .AddUdpSender(configuration)
            .AddKnx(configuration)
            .AddMediatR(m => m.RegisterServicesFromAssembly(typeof(Program).Assembly));
}