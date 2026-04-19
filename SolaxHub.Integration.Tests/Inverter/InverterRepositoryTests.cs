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

    [Theory]
    [InlineData(new byte[] { 0x00, 0x19 }, 2.5)]   // raw 25 → 2.5 kWh
    [InlineData(new byte[] { 0x01, 0x2C }, 30.0)]  // raw 300 → 30.0 kWh (old code would return 0.1)
    public async Task Given_SolarEnergyToday_Register_Should_Decode_BigEndian_U16_Scale_0_1(byte[] wireBytes, double expected)
    {
        // Arrange
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.SolarEnergyToday), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>(wireBytes)));

        var repo = BuildRepository(clientMock);

        // Act
        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        // Assert
        snapshot.Solar.EnergyToday.Should().Be(expected);
        clientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_SolarEnergyTotal_Register_Should_Decode_SwappedWord_Scale_0_1()
    {
        // reg0 (low word) = 0xA37B, reg1 (high word) = 0x0001
        // raw = 0x0001_A37B = 107387 → × 0.1 = 10738.7 kWh
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.SolarEnergyTotal), It.Is<ushort>(q => q == 2), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0xA3, 0x7B, 0x00, 0x01])));

        var repo = BuildRepository(clientMock);

        // Act
        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        // Assert
        snapshot.Solar.EnergyTotal.Should().Be(10738.7);
        clientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_FeedInEnergy_Register_Should_Decode_SwappedWord_Scale_0_01()
    {
        // reg0 (low word) = 0x2710, reg1 (high word) = 0x0000 → raw = 10000 → × 0.01 = 100.0 kWh
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.FeedInEnergy), It.Is<ushort>(q => q == 2), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0x27, 0x10, 0x00, 0x00])));

        var repo = BuildRepository(clientMock);

        // Act
        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        // Assert
        snapshot.Grid.FeedInEnergy.Should().Be(100.0);
        clientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_ConsumeEnergyTotal_Register_Should_Decode_SwappedWord_Scale_0_01()
    {
        // reg0 (low word) = 0x86A0, reg1 (high word) = 0x0001 → raw = 0x0001_86A0 = 100000 → × 0.01 = 1000.0 kWh
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.ConsumeEnergyTotal), It.Is<ushort>(q => q == 2), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0x86, 0xA0, 0x00, 0x01])));

        var repo = BuildRepository(clientMock);

        // Act
        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        // Assert
        snapshot.Grid.ConsumeEnergy.Should().Be(1000.0);
        clientMock.VerifyAll();
    }
}
