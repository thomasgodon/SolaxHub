using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Modbus.Client;

internal interface ISolaxModbusClient
{
    Task Start(CancellationToken cancellationToken);
    SolaxData? GetLastReceivedData();
    Task SetSolarChargerUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken);
}