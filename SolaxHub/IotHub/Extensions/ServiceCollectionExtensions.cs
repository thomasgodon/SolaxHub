using SolaxHub.IotHub.Models;
using SolaxHub.IotHub.Services;

namespace SolaxHub.IotHub.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIotCentral(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<IotHubOptions>(configuration.GetSection(nameof(IotHubOptions)));
            serviceCollection.AddSingleton<IIotHubDevicesService, IotHubDevicesService>();
            return serviceCollection;
        }
    }
}
