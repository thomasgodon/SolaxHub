using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolaxHub.Application.Inverter.Commands.RefreshInverterData;
using SolaxHub.Application.Inverter.Commands.SetBatteryDischargePowerTarget;
using SolaxHub.Domain.Inverter;
using SolaxHub.Integration.Tests.Fixtures;
using Xunit;

namespace SolaxHub.Integration.Tests.Inverter;

public class SetBatteryDischargePowerTargetTests
{
    /// <summary>
    /// InverterPower − FeedInPower = HouseLoad
    /// neededFromBattery = max(0, HouseLoad − pvPower1)
    /// effectiveWatts = min(command, neededFromBattery)
    /// gridWTarget = neededFromBattery − effectiveWatts
    /// </summary>
    [Theory]
    [InlineData(600, -3400, 600, 500, 2900)]   // house=4000 pv=600 → effective=500 gridWTarget=2900
    [InlineData(300, 0, 0, 500, 0)]            // house=300  pv=0   → effective=300 gridWTarget=0
    public async Task Given_HouseLoad_Exceeds_Pv_Should_Discharge_Up_To_Limit(
        int inverterPower, int feedInPower, int pvPower1, int commandWatts, int expectedGridWTarget)
    {
        // Arrange
        var fixture = new SolaxHubFixture();
        var repoMock = fixture.ServiceProvider.GetRequiredService<Mock<IInverterRepository>>();
        repoMock.Setup(r => r.ReadSnapshotAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSnapshot(inverterPower, feedInPower, pvPower1));
        repoMock.Setup(r => r.SetPowerControlAsync(It.IsAny<PowerControlMode>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sender = fixture.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new RefreshInverterDataCommand());

        // Act
        await sender.Send(new SetBatteryDischargePowerTargetCommand(commandWatts));

        // Assert
        repoMock.Verify(r => r.SetPowerControlAsync(
            PowerControlMode.PowerControlMode, expectedGridWTarget, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Given_Pv_Covers_HouseLoad_Should_Disable_PowerControl()
    {
        // Arrange — house=200W pv=600W → neededFromBattery=0 → disable
        var fixture = new SolaxHubFixture();
        var repoMock = fixture.ServiceProvider.GetRequiredService<Mock<IInverterRepository>>();
        repoMock.Setup(r => r.ReadSnapshotAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSnapshot(inverterPower: 600, feedInPower: 400, pvPower1: 600));
        repoMock.Setup(r => r.SetPowerControlAsync(It.IsAny<PowerControlMode>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sender = fixture.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new RefreshInverterDataCommand());

        // Act
        await sender.Send(new SetBatteryDischargePowerTargetCommand(500));

        // Assert
        repoMock.Verify(r => r.SetPowerControlAsync(
            PowerControlMode.Disabled, 0, It.IsAny<CancellationToken>()), Times.Once);
    }

    private static InverterSnapshot CreateSnapshot(int inverterPower, int feedInPower, int pvPower1) => new(
        SerialNumber: string.Empty,
        Status: InverterStatus.NormalMode,
        UseMode: InverterUseMode.SelfUse,
        LockState: LockState.UnlockedAdvanced,
        PowerControlMode: PowerControlMode.Disabled,
        Battery: new BatteryState(0, 50, 0, 0, 0, 0),
        Solar: new SolarState(0, 0, (ushort)pvPower1, 0, 0),
        Grid: new GridState(feedInPower, 0, 0),
        InverterPower: inverterPower,
        InverterVoltage: 230,
        RegistrationCode: string.Empty
    );
}
