using FluentAssertions;
using Moq;
using SolaxHub.Integration.Tests.Fixtures;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Queries;
using SolaxHub.Solax.Registers;
using Xunit;

namespace SolaxHub.Integration.Tests.Solax.Queries;

public class SolaxQueryTests : SolaxBaseTests
{
    public SolaxQueryTests(SolaxHubFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Given_BatteryCapacity_Requested_Should_Parse()
    {
        // Arrange
        Mock<ISolaxModbusClient> solaxModbusClientMock = Fixture.ConfigureMock<ISolaxModbusClient>(
            m => m.Setup(d => d.ReadInputRegistersAsync(
                    It.Is<ushort>(s => s == ReadInputRegisters.BatteryCapacity),
                    It.Is<ushort>(s => s == 1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0, 50])));

        // Act
        ushort result = await Sender.Send(new GetBatteryCapacityQuery(), CancellationToken.None);

        // Assert
        result.Should().Be(50);
        solaxModbusClientMock.VerifyAll();
    }

    [Theory]
    [InlineData(255, 59, -197)]
    public async Task Given_BatteryPower_Requested_Should_Parse(byte byte1, byte byte2, short expectedResult)
    {
        // Arrange
        Mock<ISolaxModbusClient> solaxModbusClientMock = Fixture.ConfigureMock<ISolaxModbusClient>(
            m => m.Setup(d => d.ReadInputRegistersAsync(
                    It.Is<ushort>(s => s == ReadInputRegisters.BatPowerCharge1),
                    It.Is<ushort>(s => s == 1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([byte1, byte2])));

        // Act
        short result = await Sender.Send(new GetBatteryPowerQuery(), CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
        solaxModbusClientMock.VerifyAll();
    }

    [Theory]
    [InlineData(0, SolaxInverterStatus.WaitMode)]
    [InlineData(1, SolaxInverterStatus.CheckMode)]
    [InlineData(2, SolaxInverterStatus.NormalMode)]
    [InlineData(4, SolaxInverterStatus.PermanentFaultMode)]
    [InlineData(5, SolaxInverterStatus.UpdateMode)]
    [InlineData(6, SolaxInverterStatus.EpsCheckMode)]
    [InlineData(7, SolaxInverterStatus.EpsMode)]
    [InlineData(8, SolaxInverterStatus.SelfTestMode)]
    [InlineData(9, SolaxInverterStatus.IdleMode)]
    [InlineData(10, SolaxInverterStatus.StandbyMode)]
    [InlineData(11, SolaxInverterStatus.PvWakeUpBatMode)]
    [InlineData(12, SolaxInverterStatus.GenCheckMode)]
    [InlineData(13, SolaxInverterStatus.GenRunMode)]
    public async Task Given_InverterStatus_Requested_Should_Parse(byte byte2, SolaxInverterStatus expectedResult)
    {
        // Arrange
        Mock<ISolaxModbusClient> solaxModbusClientMock = Fixture.ConfigureMock<ISolaxModbusClient>(
            m => m.Setup(d => d.ReadInputRegistersAsync(
                    It.Is<ushort>(s => s == ReadInputRegisters.RunMode),
                    It.Is<ushort>(s => s == 1),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Memory<byte>([0, byte2])));

        // Act
        SolaxInverterStatus result = await Sender.Send(new GetInverterStatusQuery(), CancellationToken.None);

        // Assert
        result.Should().Be(expectedResult);
        solaxModbusClientMock.VerifyAll();
    }
}