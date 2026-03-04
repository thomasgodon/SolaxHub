using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Modbus;
using SolaxHub.Infrastructure.Modbus.Client;
using SolaxHub.Infrastructure.Modbus.Options;
using SolaxHub.Infrastructure.Modbus.Registers;
using Xunit;

namespace SolaxHub.Integration.Tests.Inverter;

public class InverterRepositoryTests
{
    private static InverterRepository BuildRepository(Mock<ISolaxModbusClient> clientMock)
        => new(clientMock.Object);

    private static Mock<ISolaxModbusClient> CreateClientMock(Action<Mock<ISolaxModbusClient>>? configure = null)
    {
        var mock = new Mock<ISolaxModbusClient>();
        // Default: all reads return 14 zero bytes (sufficient for all register reads)
        mock.Setup(c => c.ReadInputRegistersAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Memory<byte>(new byte[14]));
        mock.Setup(c => c.ReadHoldingRegistersAsync(It.IsAny<ushort>(), It.IsAny<ushort>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Memory<byte>(new byte[14]));
        configure?.Invoke(mock);
        return mock;
    }

    [Fact]
    public async Task Given_BatteryCapacity_Register_Should_Parse_Correctly()
    {
        // Arrange
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.BatteryCapacity), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0, 50])));

        var repo = BuildRepository(clientMock);

        // Act
        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        // Assert
        snapshot.Battery.Capacity.Should().Be(50);
        clientMock.VerifyAll();
    }

    [Theory]
    [InlineData(255, 59, -197)]
    public async Task Given_BatteryPower_Register_Should_Parse_Correctly(byte hi, byte lo, int expected)
    {
        // Arrange
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.BatPowerCharge1), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([hi, lo])));

        var repo = BuildRepository(clientMock);

        // Act
        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        // Assert
        snapshot.Battery.Power.Should().Be(expected);
        clientMock.VerifyAll();
    }

    [Theory]
    [InlineData(0, InverterStatus.WaitMode)]
    [InlineData(1, InverterStatus.CheckMode)]
    [InlineData(2, InverterStatus.NormalMode)]
    [InlineData(4, InverterStatus.PermanentFaultMode)]
    [InlineData(9, InverterStatus.IdleMode)]
    [InlineData(13, InverterStatus.GenRunMode)]
    public async Task Given_RunMode_Register_Should_Parse_InverterStatus(byte rawValue, InverterStatus expected)
    {
        // Arrange
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.RunMode), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0, rawValue])));

        var repo = BuildRepository(clientMock);

        // Act
        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        // Assert
        snapshot.Status.Should().Be(expected);
        clientMock.VerifyAll();
    }

    [Theory]
    [InlineData(0, LockState.Locked)]
    [InlineData(1, LockState.Unlocked)]
    [InlineData(2, LockState.UnlockedAdvanced)]
    public async Task Given_LockState_Register_Should_Parse_Correctly(byte rawValue, LockState expected)
    {
        // Arrange
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.LockState), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0, rawValue])));

        var repo = BuildRepository(clientMock);

        // Act
        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        // Assert
        snapshot.LockState.Should().Be(expected);
        clientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_SolarEnergyToday_Register_Should_Scale_By_0_1()
    {
        // Arrange
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.SolarEnergyToday), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([25, 0])));

        var repo = BuildRepository(clientMock);

        // Act
        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        // Assert
        snapshot.Solar.EnergyToday.Should().Be(2.5);
        clientMock.VerifyAll();
    }
}
