using SolaxHub.IotCentral;
using SolaxHub.Knx.Client;
using SolaxHub.Solax;

namespace SolaxHub.Knx.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddKnx(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<KnxOptions>(configuration.GetSection(nameof(KnxOptions)));
            serviceCollection.AddSingleton<ISolaxProcessor, KnxSolaxProcessor>();
            serviceCollection.AddSingleton<IKnxClient, KnxClient>();
            serviceCollection.AddSingleton<ISolaxWriter, KnxSolaxWriter>();
            return serviceCollection;
        }
    }
}
