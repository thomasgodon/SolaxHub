using System.IO.Ports;
using SolaxHub.Solax.Http;

namespace SolaxHub.Solax.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSolaxClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<SolaxHttpOptions>(configuration.GetSection(nameof(SolaxHttpOptions)));
            serviceCollection.AddSingleton<ISolaxClient, SolaxHttpClient>();
            serviceCollection.AddSingleton<ISolaxProcessorService, SolaxProcessorService>();
            serviceCollection.AddSingleton<SerialPort>();

            return serviceCollection;
        }
    }
}
