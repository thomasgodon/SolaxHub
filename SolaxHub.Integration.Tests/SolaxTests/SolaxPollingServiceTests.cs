using Moq;
using SolaxHub.Integration.Tests.Fixtures;
using SolaxHub.Integration.Tests.SolaxTests.Base;
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
                        It.Is<ushort>(s => s == ReadInputRegisters.LockState),
                        It.Is<ushort>(s => s == 1),
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Memory<byte>([0, 0]));

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
}