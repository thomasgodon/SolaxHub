using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolaxHub.Application.Inverter.Services;
using SolaxHub.Application.PowerControl;

namespace SolaxHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(m => m.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddSingleton<IInverterStateService, InverterStateService>();
        services.Configure<PowerControlOptions>(configuration.GetSection(nameof(PowerControlOptions)));
        return services;
    }
}
