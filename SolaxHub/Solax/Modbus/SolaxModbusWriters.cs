namespace SolaxHub.Solax.Modbus;

internal partial class SolaxModbusClient
{
    public async Task SetSolarChargerUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken)
    {
        const ushort registerAddress = 0x001F;
        var data = BitConverter.GetBytes((ushort)useMode);

        await _modbusClient.WriteSingleRegisterAsync(UnitIdentifier, registerAddress, data, cancellationToken);
    }
}