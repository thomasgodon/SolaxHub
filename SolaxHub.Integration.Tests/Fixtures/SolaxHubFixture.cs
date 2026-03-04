using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolaxHub.Domain.Inverter;
using SolaxHub.Extensions;
using SolaxHub.Infrastructure.Knx.Client;
using SolaxHub.Integration.Tests.Fixtures.Extensions;

namespace SolaxHub.Integration.Tests.Fixtures;

public class SolaxHubFixture
{
    private readonly ServiceCollection _serviceCollection = [];

    public ServiceProvider ServiceProvider { get; }

    public SolaxHubFixture()
    {
        IConfigurationBuilder configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
        IConfiguration configuration = configurationBuilder.Build();

        _serviceCollection.AddLogging();
        _serviceCollection.AddSingleton(configuration);
        _serviceCollection.AddSolaxHub(configuration);

        _serviceCollection.ReplaceWithMock<IInverterRepository>(ServiceLifetime.Singleton);
        _serviceCollection.ReplaceWithMock<IKnxClient>(ServiceLifetime.Singleton);

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
