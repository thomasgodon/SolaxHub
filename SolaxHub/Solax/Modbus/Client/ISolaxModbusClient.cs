using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Modbus.Client;

public interface ISolaxModbusClient
{
    Task Start(CancellationToken cancellationToken);
    Task SetSolarChargerUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken);
}