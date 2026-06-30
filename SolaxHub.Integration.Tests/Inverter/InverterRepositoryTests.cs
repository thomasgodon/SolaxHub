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

    [Fact]
    public async Task Given_PvPower2_Register_Should_Decode_BigEndian_U16()
    {
        // raw 0x0BB8 = 3000 W
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.PowerDc2), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0x0B, 0xB8])));

        var repo = BuildRepository(clientMock);

        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        snapshot.Solar.Power2.Should().Be(3000);
        clientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_BatteryVoltage_Register_Should_Decode_S16_Scale_0_01()
    {
        // raw 0x1400 = 5120 → /100 = 51.20 V (handler scales; repo stores raw 0.01V units)
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.BatteryVoltage), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0x14, 0x00])));

        var repo = BuildRepository(clientMock);

        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        snapshot.Battery.Voltage.Should().Be(5120);
        clientMock.VerifyAll();
    }

    [Theory]
    [InlineData(0x00, 0x32, (short)50)]    // raw 50 → 5.0 A
    [InlineData(0xFF, 0x9C, (short)-100)]  // raw -100 → -10.0 A (discharging)
    public async Task Given_BatteryCurrent_Register_Should_Decode_S16(byte hi, byte lo, short expected)
    {
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.BatteryCurrent), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([hi, lo])));

        var repo = BuildRepository(clientMock);

        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        snapshot.Battery.Current.Should().Be(expected);
        clientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_BatteryTemperature_Register_Should_Decode_U16()
    {
        // raw 0x00FA = 250 → /10 = 25.0 °C (handler scales; repo stores raw 0.1°C units)
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.BatteryTemperature), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0x00, 0xFA])));

        var repo = BuildRepository(clientMock);

        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        snapshot.Battery.Temperature.Should().Be(250);
        clientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_GridFrequency_Register_Should_Decode_U16()
    {
        // raw 0x1388 = 5000 → /100 = 50.00 Hz (handler scales; repo stores raw 0.01Hz units)
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.GridFrequency), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0x13, 0x88])));

        var repo = BuildRepository(clientMock);

        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        snapshot.Grid.Frequency.Should().Be(5000);
        clientMock.VerifyAll();
    }

    [Theory]
    [InlineData(0x00, 0x28, (short)40)]    // raw 40 → 40 °C
    [InlineData(0xFF, 0xFB, (short)-5)]    // raw -5 → -5 °C
    public async Task Given_InverterTemperature_Register_Should_Decode_Signed(byte hi, byte lo, short expected)
    {
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.InverterTemperature), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([hi, lo])));

        var repo = BuildRepository(clientMock);

        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        snapshot.InverterTemperature.Should().Be(expected);
        clientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_EpsFrequency_Register_Should_Decode_U16()
    {
        // raw 0x1388 = 5000 → /100 = 50.00 Hz
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.EpsFrequency), It.Is<ushort>(q => q == 1), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0x13, 0x88])));

        var repo = BuildRepository(clientMock);

        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        snapshot.Eps.Frequency.Should().Be(5000);
        clientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_InverterFault_Register_Should_Decode_SwappedWord_U32()
    {
        // reg0 (low word) = 0x0002, reg1 (high word) = 0x0001 → raw = 0x0001_0002 = 65538
        var clientMock = CreateClientMock(m =>
            m.Setup(c => c.ReadInputRegistersAsync(
                    It.Is<ushort>(a => a == InputRegisters.InverterFault), It.Is<ushort>(q => q == 2), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0x00, 0x02, 0x00, 0x01])));

        var repo = BuildRepository(clientMock);

        var snapshot = await repo.ReadSnapshotAsync(CancellationToken.None);

        snapshot.Faults.InverterFault.Should().Be(65538u);
        snapshot.Faults.HasFault.Should().BeTrue();
        clientMock.VerifyAll();
    }
}
