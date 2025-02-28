using Moq;
using SolaxHub.Integration.Tests.Fixtures;
using SolaxHub.Integration.Tests.SolaxTests.Base;
using SolaxHub.Knx.Client;
using SolaxHub.Knx.Models;
using SolaxHub.Solax.Modbus.Client;
using SolaxHub.Solax.Registers;
using Xunit;

namespace SolaxHub.Integration.Tests.SolaxTests;

public class SolaxPollingServiceTests : SolaxBaseTests
{
    public SolaxPollingServiceTests(SolaxHubFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task Given_Inverter_LockState_Not_UnlockAdvanced_Should_UnlockedAdvanced()
    {
        // Arrange
        Mock<ISolaxModbusClient> solaxModbusClientMock = Fixture.ConfigureMock<ISolaxModbusClient>(
            m =>
            {
                m.Setup(d => d.ReadInputRegistersAsync(
                        It.IsAny<ushort>(),
                        It.IsAny<ushort>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Memory<byte>("\0\0\0\0\0\0\0\0\0\0\0\0"u8.ToArray()));

                m.Setup(d => d.ReadInputRegistersAsync(
                        It.Is<ushort>(s => s == ReadInputRegisters.LockState),
                        It.Is<ushort>(s => s == 1),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Memory<byte>("\0\0"u8.ToArray()));

                m.Setup(d => d.WriteSingleRegisterAsync(
                    It.Is<ushort>(s => s == WriteSingleRegisters.UnlockPassword),
                    It.Is<byte[]>(s => s.SequenceEqual(new byte[] { 26, 212 })),
                    It.IsAny<CancellationToken>()));
            });

        // Act
        await SolaxPollingService.ProcessAsync(CancellationToken.None);

        // Assert
        solaxModbusClientMock.VerifyAll();
    }

    [Fact]
    public async Task Given_SolarEnergyToday_Has_Value_Should_Send_Knx_Value_SolarEnergyToday()
    {
        // Arrange
        Mock<ISolaxModbusClient> solaxModbusClientMock = Fixture.ConfigureMock<ISolaxModbusClient>(
            m =>
            {
                m.Setup(d => d.ReadInputRegistersAsync(
                        It.IsAny<ushort>(),
                        It.IsAny<ushort>(),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Memory<byte>("\0\0\0\0\0\0\0\0\0\0\0\0"u8.ToArray()));

                m.Setup(d => d.ReadInputRegistersAsync(
                        It.Is<ushort>(s => s == ReadInputRegisters.SolarEnergyToday),
                        It.Is<ushort>(s => s == 1),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Memory<byte>([25, 126]));
            });

        Mock<IKnxClient> knxClientMock = Fixture.ConfigureMock<IKnxClient>(
            m => m.Setup(d => d.SendValuesAsync(
                It.IsAny<IEnumerable<KnxValue>>(),
                It.IsAny<CancellationToken>())));

        // Act
        await SolaxPollingService.ProcessAsync(CancellationToken.None);

        // Assert
        solaxModbusClientMock.VerifyAll();
        knxClientMock.VerifyAll();
    }
}