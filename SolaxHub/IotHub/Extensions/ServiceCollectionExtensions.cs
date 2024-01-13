using SolaxHub.Solax;

namespace SolaxHub.IotHub.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIotCentral(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<IotHubOptions>(configuration.GetSection(nameof(IotHubOptions)));
            serviceCollection.AddSingleton<ISolaxConsumer, IotHubSolaxConsumerService>();
            return serviceCollection;
        }
    }
}
