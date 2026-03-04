using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SolaxHub.Integration.Tests.Fixtures;
using Xunit;

namespace SolaxHub.Integration.Tests.Inverter;

public abstract class InverterBaseTests : IClassFixture<SolaxHubFixture>
{
    public SolaxHubFixture Fixture { get; }

    public ISender Sender => Fixture.ServiceProvider.GetRequiredService<ISender>();

    protected InverterBaseTests(SolaxHubFixture fixture)
    {
        Fixture = fixture;
    }
}
