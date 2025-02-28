using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SolaxHub.Integration.Tests.Fixtures;
using SolaxHub.Solax.Services;
using Xunit;

namespace SolaxHub.Integration.Tests.Solax;

public abstract class SolaxBaseTests : IClassFixture<SolaxHubFixture>
{
    public SolaxHubFixture Fixture { get; }

    public ISolaxPollingService SolaxPollingService => Fixture.ServiceProvider.GetService<ISolaxPollingService>() ?? throw new InvalidOperationException($"Could not resolve {nameof(ISolaxPollingService)}");
    public ISender Sender => Fixture.ServiceProvider.GetService<ISender>() ?? throw new InvalidOperationException($"Could not resolve {nameof(ISender)}");

    protected SolaxBaseTests(SolaxHubFixture fixture)
    {
        Fixture = fixture;
    }
}