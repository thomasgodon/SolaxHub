using SolaxHub.Solax;

namespace SolaxHub.Udp.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUdpSender(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<UdpOptions>(configuration.GetSection(nameof(UdpOptions)));
            serviceCollection.AddTransient<ISolaxConsumer, UdpSolaxConsumerService>();
            return serviceCollection;
        }
    }
}
