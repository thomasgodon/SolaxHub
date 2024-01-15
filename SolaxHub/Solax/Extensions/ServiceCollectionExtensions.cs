using SolaxHub.Solax.Modbus;
using SolaxHub.Solax.Services;

namespace SolaxHub.Solax.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSolaxClients(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddModbusClient(configuration);
            serviceCollection.AddSingleton<ISolaxProcessorService, SolaxProcessorService>();

            return serviceCollection;
        }

        private static void AddModbusClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<SolaxModbusOptions>(configuration.GetSection(nameof(SolaxModbusOptions)));
            serviceCollection.AddSingleton<ISolaxModbusClient, SolaxModbusClient>();
        }
    }
}
