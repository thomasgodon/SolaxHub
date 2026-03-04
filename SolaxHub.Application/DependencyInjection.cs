using Microsoft.Extensions.DependencyInjection;
using SolaxHub.Application.Inverter.Services;

namespace SolaxHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(m => m.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
        services.AddSingleton<IInverterStateService, InverterStateService>();
        return services;
    }
}
