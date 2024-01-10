using SolaxHub.Solax.Modbus;

namespace SolaxHub.Solax.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSolaxClients(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddSingleton<ISolaxClientFactory, SolaxClientFactory>();
            serviceCollection.AddModbusClient(configuration);
            serviceCollection.AddSingleton<ISolaxProcessorService, SolaxProcessorService>();

            return serviceCollection;
        }

        private static void AddModbusClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<SolaxModbusOptions>(configuration.GetSection(nameof(SolaxModbusOptions)));
            serviceCollection.AddSingleton<SolaxModbusClient>();
            serviceCollection.AddSingleton<ISolaxModbusClient>(m => m.GetRequiredService<SolaxModbusClient>());
        }
    }
}
