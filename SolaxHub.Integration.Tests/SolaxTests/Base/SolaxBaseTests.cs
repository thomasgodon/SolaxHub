using Microsoft.Extensions.DependencyInjection;
using SolaxHub.Integration.Tests.Fixtures;
using SolaxHub.Solax.Services;
using Xunit;

namespace SolaxHub.Integration.Tests.SolaxTests.Base;

public abstract class SolaxBaseTests : IClassFixture<SolaxHubFixture>
{
    private readonly SolaxHubFixture _fixture;

    public SolaxHubFixture Fixture => _fixture;
    public ISolaxPollingService SolaxPollingService => _fixture.ServiceProvider.GetService<ISolaxPollingService>() ?? throw new InvalidOperationException("Could not resolve ISolaxPollingService");

    protected SolaxBaseTests(SolaxHubFixture fixture)
    {
        _fixture = fixture;
    }
}