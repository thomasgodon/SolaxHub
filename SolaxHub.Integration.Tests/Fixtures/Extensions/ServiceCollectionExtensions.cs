using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace SolaxHub.Integration.Tests.Fixtures.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ReplaceWithMock<TService>(this IServiceCollection services, ServiceLifetime lifetime) where TService : class
    {
        ArgumentNullException.ThrowIfNull(services);

        // Swap the actual registered service with a factory, that will return the mock.
        ServiceDescriptor descriptor = lifetime switch
        {
            // NOTE: On singleton the lifetime of a mock can already contain setup(s).
            ServiceLifetime.Singleton => ServiceDescriptor.Singleton(provider => provider.GetRequiredService<Mock<TService>>().Object),
            ServiceLifetime.Scoped => ServiceDescriptor.Scoped(provider => provider.GetRequiredService<Mock<TService>>().Object),
            ServiceLifetime.Transient => ServiceDescriptor.Transient(provider => provider.GetRequiredService<Mock<TService>>().Object),
            _ => throw new NotSupportedException(),
        };

        services.Replace(descriptor);
        services.AddSingleton<Mock<TService>>();

        // The untyped version just calls the typed one. To resolve all Mock(s).
        services.AddSingleton<Mock>(provider
            => provider.GetRequiredService<Mock<TService>>());

        return services;
    }
}