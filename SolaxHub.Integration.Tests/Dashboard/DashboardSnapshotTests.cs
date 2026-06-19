using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SolaxHub.Application.Dashboard;
using SolaxHub.Application.Dashboard.Options;
using SolaxHub.Application.Inverter.Notifications;
using SolaxHub.Integration.Tests.Fixtures;
using Xunit;

namespace SolaxHub.Integration.Tests.Dashboard;

public class DashboardSnapshotTests
{
    [Fact]
    public async Task Given_Dashboard_Enabled_When_Inverter_Data_Refreshed_Then_Broadcaster_Publishes_Snapshot()
    {
        // Arrange
        var fixture = new SolaxHubFixture(services =>
            services.Configure<DashboardOptions>(o => o.Enabled = true));

        var publisher = fixture.ServiceProvider.GetRequiredService<IPublisher>();
        var broadcaster = fixture.ServiceProvider.GetRequiredService<IInverterSnapshotBroadcaster>();

        // Act
        await publisher.Publish(new InverterDataRefreshed(SolaxHub.Domain.Inverter.Inverter.Create()));

        // Assert
        broadcaster.Latest.Should().NotBeNull();
        broadcaster.Latest.Should().Contain("\"connections\"");
        broadcaster.Latest.Should().Contain("\"battery\"");
    }

    [Fact]
    public async Task Given_Dashboard_Disabled_When_Inverter_Data_Refreshed_Then_Broadcaster_Stays_Empty()
    {
        // Arrange — DashboardOptions.Enabled defaults to false (no override).
        var fixture = new SolaxHubFixture();

        var publisher = fixture.ServiceProvider.GetRequiredService<IPublisher>();
        var broadcaster = fixture.ServiceProvider.GetRequiredService<IInverterSnapshotBroadcaster>();

        // Act
        await publisher.Publish(new InverterDataRefreshed(SolaxHub.Domain.Inverter.Inverter.Create()));

        // Assert
        broadcaster.Latest.Should().BeNull();
    }
}
