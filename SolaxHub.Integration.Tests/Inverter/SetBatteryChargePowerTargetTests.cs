using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SolaxHub.Application.Inverter.Commands.RefreshInverterData;
using SolaxHub.Application.Inverter.Commands.SetBatteryChargePowerTarget;
using SolaxHub.Application.PowerControl;
using SolaxHub.Domain.Inverter;
using SolaxHub.Integration.Tests.Fixtures;
using Xunit;

namespace SolaxHub.Integration.Tests.Inverter;

public class SetBatteryChargePowerTargetTests
{
    private const int MaxGridImport = 4500;

    /// <summary>
    /// effectiveCharge = max(0, min(Watts, MaxGridImport + PV − HouseLoad))
    /// gridWTarget = max(0, house−pv) + max(0, effectiveCharge − max(0, pv−house))
    /// </summary>
    [Theory]
    [InlineData(0, 0, 2500, 3000, 500)]        // house=0   pv=2500 → pvSurplus=2500 → gridForBat=500 → gridWTarget=500
    [InlineData(600, -3400, 2500, 3000, 4500)]  // house=4000 pv=2500 → gridForHouse=1500 gridForBat=3000 → gridWTarget=4500
    [InlineData(600, -5400, 2500, 3000, 4500)]  // house=6000 pv=2500 → effective=1000 gridForHouse=3500 gridForBat=1000 → gridWTarget=4500
    public async Task Given_HouseLoad_Should_Charge_With_Grid_Headroom(
        int inverterPower, int feedInPower, int pvPower1, int commandWatts, int expectedGridWTarget)
    {
        // Arrange
        var fixture = new SolaxHubFixture();
        fixture.ServiceProvider.GetRequiredService<IPowerControlStateService>().SetMaxGridImportWatts(MaxGridImport);
        var repoMock = fixture.ServiceProvider.GetRequiredService<Mock<IInverterRepository>>();
        repoMock.Setup(r => r.ReadSnapshotAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSnapshot(inverterPower, feedInPower, pvPower1));
        repoMock.Setup(r => r.SetPowerControlAsync(It.IsAny<PowerControlMode>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sender = fixture.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new RefreshInverterDataCommand());

        // Act
        await sender.Send(new SetBatteryChargePowerTargetCommand(commandWatts));

        // Assert
        repoMock.Verify(r => r.SetPowerControlAsync(
            PowerControlMode.PowerControlMode, expectedGridWTarget, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Given_HouseLoad_Exceeds_MaxGrid_Should_Disable_PowerControl()
    {
        // Arrange — house=10000W pv=2500W → headroom = 4500+2500-10000 = -3000 → effective=0 → disable
        var fixture = new SolaxHubFixture();
        fixture.ServiceProvider.GetRequiredService<IPowerControlStateService>().SetMaxGridImportWatts(MaxGridImport);
        var repoMock = fixture.ServiceProvider.GetRequiredService<Mock<IInverterRepository>>();
        repoMock.Setup(r => r.ReadSnapshotAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateSnapshot(inverterPower: 2500, feedInPower: -7500, pvPower1: 2500));
        repoMock.Setup(r => r.SetPowerControlAsync(It.IsAny<PowerControlMode>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var sender = fixture.ServiceProvider.GetRequiredService<ISender>();
        await sender.Send(new RefreshInverterDataCommand());

        // Act
        await sender.Send(new SetBatteryChargePowerTargetCommand(3000));

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
