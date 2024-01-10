using SolaxHub.Solax;

namespace SolaxHub.IotCentral.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIotCentral(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<IotCentralOptions>(configuration.GetSection(nameof(IotCentralOptions)));
            serviceCollection.AddSingleton<ISolaxConsumer, IotCentralPublisherService>();
            return serviceCollection;
        }
    }
}
