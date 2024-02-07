using System.Buffers.Binary;
using System.Diagnostics;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Modbus.Client;

internal partial class SolaxModbusClient
{
    private readonly Stopwatch _powerControlWatch = new();
    private static readonly TimeSpan PowerControlInterval = TimeSpan.FromSeconds(5);

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
        var dataSetType = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(Convert.ToUInt16(1)));
        var dataSetActivePower = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)activePower));
        var dataSetReactivePower = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)reactivePower));
        var dataSetTimeout = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(Convert.ToUInt16(20)));

        var dataSet = dataSetEnabled
            .Concat(dataSetActivePower)
            .Concat(dataSetReactivePower)
            .Concat(dataSetTimeout)
            .ToArray();
        _logger.LogInformation(BitConverter.ToString(dataSet));
        await _modbusClient.WriteMultipleRegistersAsync(UnitIdentifier, registerAddress, dataSet, cancellationToken);
    }

    private bool ShouldSendPowerControl()
    {
        if (_powerControlWatch.IsRunning is false)
        {
            _powerControlWatch.Start();
            return true;
        }

        if (_powerControlWatch.Elapsed >= PowerControlInterval)
        {
            _powerControlWatch.Restart();
            return true;
        }

        return false;
    }
}