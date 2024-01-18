using System.Buffers.Binary;
using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Modbus.Client;

internal partial class SolaxModbusClient
{
    public async Task SetSolarChargerUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x001F;
        await _modbusClient.WriteSingleRegisterAsync(UnitIdentifier, registerAddress, (ushort)useMode, cancellationToken);
    }

    public async Task SetLockStateAsync(SolaxLockState lockState, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x0000;
        await _modbusClient.WriteSingleRegisterAsync(UnitIdentifier, registerAddress, (ushort)lockState, cancellationToken);
    }

    public async Task SetPowerControlAsync(bool enabled, double activePower, double reactivePower, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x7C;

        var dataSetEnabled = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness(Convert.ToInt16(enabled)));
        var dataSetActivePower = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)activePower));
        var dataSetReactivePower = BitConverter.GetBytes(BinaryPrimitives.ReverseEndianness((int)reactivePower));

        var dataSet = dataSetEnabled
            .Concat(dataSetActivePower)
            .Concat(dataSetReactivePower)
            .ToArray();

        await _modbusClient.WriteMultipleRegistersAsync(UnitIdentifier, registerAddress, dataSet, cancellationToken);
    }
}