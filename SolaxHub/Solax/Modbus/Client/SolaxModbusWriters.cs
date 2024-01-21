using System.Buffers.Binary;
using System.Diagnostics;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Modbus.Client;

internal partial class SolaxModbusClient
{
    private readonly Stopwatch _powerControlWatch = new();

    public async Task SetSolarChargerUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x001F;
        await _modbusClient.WriteSingleRegisterAsync(UnitIdentifier, registerAddress, (ushort)useMode, cancellationToken);
    }

    public async Task SetLockStateAsync(SolaxLockState lockState, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x0000;
        await _modbusClient.WriteSingleRegisterAsync(UnitIdentifier, registerAddress, BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((ushort)lockState)), cancellationToken);
    }

    public async Task SetPowerControlAsync(bool enabled, double activePower, double reactivePower, SolaxData data, CancellationToken cancellationToken)
    {
        if (ShouldSendPowerControl() is false && enabled)
        {
            return;
        }

        const ushort registerAddress = 0x7C;

        var dataSetEnabled = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(Convert.ToInt16(enabled)));
        var dataSetActivePower = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)activePower));
        var dataSetReactivePower = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)reactivePower));

        var dataSet = dataSetEnabled
            .Concat(dataSetActivePower)
            .Concat(dataSetReactivePower)
            .ToArray();

        await _modbusClient.WriteMultipleRegistersAsync(UnitIdentifier, registerAddress, dataSet, cancellationToken);
        _logger.LogInformation("Enabled: {enabled}, active power: {activePower}", enabled, activePower);
        _logger.LogInformation("Timeout: {timeout}", data.PowerControlTimeout);
    }

    private bool ShouldSendPowerControl()
    {
        if (_powerControlWatch.IsRunning is false)
        {
            _powerControlWatch.Start();
            return true;
        }

        if (_powerControlWatch.Elapsed >= TimeSpan.FromSeconds(120))
        {
            _powerControlWatch.Restart();
            return true;
        }

        return false;
    }
}