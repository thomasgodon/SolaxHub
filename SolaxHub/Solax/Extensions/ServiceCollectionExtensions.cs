using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Modbus.Models;
using SolaxHub.Solax.Services;
using SolaxModbusClient = SolaxHub.Solax.Modbus.Client.SolaxModbusClient;

namespace SolaxHub.Solax.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSolaxClients(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddModbusClient(configuration);
            serviceCollection.AddSingleton<ISolaxControllerService, SolaxControllerService>();

            return serviceCollection;
        }

        private static void AddModbusClient(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<SolaxModbusOptions>(configuration.GetSection(nameof(SolaxModbusOptions)));
            serviceCollection.AddSingleton<ISolaxModbusClient, SolaxModbusClient>();
        }
    }
}
