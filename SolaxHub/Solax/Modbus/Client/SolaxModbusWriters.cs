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

    public async Task SetModbusPowerControlAsync(bool enabled, double activePower, double reactivePower, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x007C;
        var dataSet = GetPowerControlDataSet(enabled, (short)activePower, (short)reactivePower);
        await _modbusClient.WriteMultipleRegistersAsync(UnitIdentifier, registerAddress, dataSet, cancellationToken);
    }

    private static byte[] GetPowerControlDataSet(bool enabled, short activePower, short reactivePower)
    {
        var enabledValue = BitConverter.GetBytes(enabled);
        var activePowerValue = BitConverter.GetBytes(activePower);
        var reactivePowerValue = BitConverter.GetBytes(reactivePower);

        var dataSet = new byte[enabledValue.Length + activePowerValue.Length + reactivePowerValue.Length];
        Buffer.BlockCopy(enabledValue, 0, dataSet, 0, enabledValue.Length);
        Buffer.BlockCopy(activePowerValue, 0, dataSet, enabledValue.Length, activePowerValue.Length);
        Buffer.BlockCopy(reactivePowerValue, 0, dataSet, enabledValue.Length + activePowerValue.Length, reactivePowerValue.Length);

        return dataSet;
    }
}