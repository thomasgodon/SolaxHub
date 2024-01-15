using SolaxHub.Knx.Client;

namespace SolaxHub.Knx.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKnx(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<KnxOptions>(configuration.GetSection(nameof(KnxOptions)));
            serviceCollection.AddSingleton<IKnxClient, KnxClient>();
            return serviceCollection;
        }
    }
}
