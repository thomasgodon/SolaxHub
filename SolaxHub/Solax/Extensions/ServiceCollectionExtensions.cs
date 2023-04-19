using System.IO.Ports;
using SolaxHub.Solax.Http;
using SolaxHub.Solax.Modbus;

namespace SolaxHub.Solax.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSolaxClients(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<ISolaxClientFactory, SolaxClientFactory>();
            serviceCollection.AddHttpClient(configuration);
            serviceCollection.AddModbusClient(configuration);
            serviceCollection.AddSingleton<ISolaxProcessorService, SolaxProcessorService>();

            return serviceCollection;
        }

        private static void AddHttpClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<SolaxHttpOptions>(configuration.GetSection(nameof(SolaxHttpOptions)));
            serviceCollection.AddSingleton<SolaxHttpClient>();
            serviceCollection.AddSingleton<ISolaxClient>(m => m.GetRequiredService<SolaxHttpClient>());
        }

        private static void AddModbusClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<SolaxModbusOptions>(configuration.GetSection(nameof(SolaxModbusOptions)));
            serviceCollection.AddSingleton<SolaxModbusClient>();
            serviceCollection.AddSingleton<ISolaxClient>(m => m.GetRequiredService<SolaxModbusClient>());
        }
    }
}
