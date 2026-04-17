using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SolaxHub.Application.Inverter.Services;
using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Knx.Client;
using SolaxHub.Infrastructure.Knx.Options;
using SolaxHub.Infrastructure.Knx.Services;
using SolaxHub.Infrastructure.Knx.Workers;
using SolaxHub.Infrastructure.Modbus;
using SolaxHub.Infrastructure.Modbus.Client;
using SolaxHub.Infrastructure.Modbus.Options;
using SolaxHub.Infrastructure.Udp.Options;

namespace SolaxHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(m => m.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));

        // Modbus / Inverter
        services.Configure<ModbusOptions>(configuration.GetSection("ModbusOptions"));
        services.AddSingleton<ISolaxModbusClient, SolaxModbusClient>();
        services.AddSingleton<IInverterRepository, InverterRepository>();
        services.AddSingleton<IInverterCommandQueue, InverterCommandQueue>();

        // KNX
        services.Configure<KnxOptions>(configuration.GetSection(nameof(KnxOptions)));
        services.AddSingleton<IKnxClient, KnxClient>();
        services.AddSingleton<IKnxValueBufferService, KnxValueBufferService>();
        services.AddHostedService<KnxConnectionWorker>();

        // UDP
        services.Configure<UdpOptions>(configuration.GetSection(nameof(UdpOptions)));

        return services;
    }
}
