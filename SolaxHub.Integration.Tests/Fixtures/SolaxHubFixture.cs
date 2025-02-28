using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolaxHub.Extensions;
using SolaxHub.Integration.Tests.Fixtures.Extensions;
using SolaxHub.IotHub.Services;
using SolaxHub.Knx.Client;
using SolaxHub.Solax.Modbus.Client;

namespace SolaxHub.Integration.Tests.Fixtures;

public class SolaxHubFixture
{
    private readonly ServiceCollection _serviceCollection = [];

    public ServiceProvider ServiceProvider { get; }

    public SolaxHubFixture()
    {
        // Build configuration
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        IConfiguration configuration = configurationBuilder.Build();

        _serviceCollection.AddSingleton(configuration);
        _serviceCollection.AddSolaxHub(configuration);

        _serviceCollection.ReplaceWithMock<ISolaxModbusClient>(ServiceLifetime.Singleton);
        _serviceCollection.ReplaceWithMock<IKnxClient>(ServiceLifetime.Singleton);
        _serviceCollection.ReplaceWithMock<IIotHubDevicesService>(ServiceLifetime.Singleton);

        ServiceProvider = _serviceCollection.BuildServiceProvider();
    }

    public Mock<T> ConfigureMock<T>(Action<Mock<T>> configure) where T : class
    {
        ArgumentNullException.ThrowIfNull(configure);

        Mock<T> mock = ServiceProvider.GetRequiredService<Mock<T>>();
        configure(mock);
        return mock;
    }
}
