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
        if (!ShouldSendPowerControl() || !enabled)
        {
            return;
        }

        activePower = 1000;

        const ushort registerAddress = 0x7C;

        var dataSetType = BitConverter.GetBytes(Convert.ToUInt16(0)).Reverse();
        var dataSetEnabled = BitConverter.GetBytes(Convert.ToUInt16(enabled ? 16 : 0)).Reverse();
        var dataSetActivePower = ReverseBits(BitConverter.GetBytes(Convert.ToInt32(activePower))).Reverse();
        var dataSetReactivePower = ReverseBits(BitConverter.GetBytes(Convert.ToInt32(reactivePower))).Reverse();
        var dataSetTimeout = BitConverter.GetBytes(Convert.ToUInt16(20)).Reverse();

        var dataSet = dataSetEnabled
            .Concat(dataSetActivePower)
            .Concat(dataSetReactivePower)
            .ToArray();
        _logger.LogInformation(BitConverter.ToString(dataSet));
        await _modbusClient.WriteMultipleRegistersAsync(UnitIdentifier, registerAddress, dataSet, cancellationToken);
    }

    private bool ShouldSendPowerControl()
    {
        if (!_powerControlWatch.IsRunning)
        {
            _powerControlWatch.Start();
            return true;
        }

        if (_powerControlWatch.Elapsed < PowerControlInterval)
        {
            return false;
        }

        _powerControlWatch.Restart();
        return true;

    }

    private static byte[] ReverseBits(byte[] data)
    {
        if (data.Length != 4)
        {
            return data;
        }
        var reversed = new byte[data.Length];
        Buffer.BlockCopy(data, 0, reversed, 2, 2);
        Buffer.BlockCopy(data, 2, reversed, 0, 2);
        return reversed;
    }
}