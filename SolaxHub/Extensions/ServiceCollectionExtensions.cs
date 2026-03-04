using SolaxHub.Application;
using SolaxHub.Infrastructure;
using SolaxHub.Workers;

namespace SolaxHub.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSolaxHub(this IServiceCollection services, IConfiguration configuration)
        => services
            .AddApplication()
            .AddInfrastructure(configuration)
            .AddHostedService<InverterPollingWorker>();
}
