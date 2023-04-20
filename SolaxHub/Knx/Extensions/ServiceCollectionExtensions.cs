using SolaxHub.IotCentral;
using SolaxHub.Solax;

namespace SolaxHub.Knx.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKnx(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<KnxOptions>(configuration.GetSection(nameof(KnxOptions)));
            serviceCollection.AddTransient<ISolaxProcessor, KnxProcessor>();
            return serviceCollection;
        }
    }
}
