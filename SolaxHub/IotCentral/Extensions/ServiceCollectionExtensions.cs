namespace SolaxHub.IotCentral.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIotCentral(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.Configure<UdpOptions>(configuration.GetSection(nameof(UdpOptions)));
            serviceCollection.AddTransient<IDsmrProcessor, UdpProcessor>();
            return serviceCollection;
        }
    }
}
