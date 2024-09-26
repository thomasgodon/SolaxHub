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

    public async Task SetPowerControlAsync(SolaxPowerControlMode mode, byte[] data, CancellationToken cancellationToken)
    {
        if (!ShouldSendPowerControl())
        {
            return;
        }

        const ushort registerAddress = 0x7C;
        
        _logger.LogTrace(BitConverter.ToString(data));
        await _modbusClient.WriteMultipleRegistersAsync(UnitIdentifier, registerAddress, data, cancellationToken);
    }

    public async Task SetBatteryDischargeMaxCurrent(double maxCurrent, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x0025;
        
        // Scale the double value to fit into an uint16 range
        // Adjust the scale factor depending on the expected range
        ushort scaledValue = (ushort)Math.Clamp(maxCurrent, 0, ushort.MaxValue); // Clamps between 0 and 65535

        await _modbusClient.WriteSingleRegisterAsync(UnitIdentifier, registerAddress, scaledValue, cancellationToken);
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
}