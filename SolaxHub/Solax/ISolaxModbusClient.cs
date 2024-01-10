using SolaxHub.Solax.Models;

namespace SolaxHub.Solax;

public interface ISolaxModbusClient
{
    Task Start(CancellationToken cancellationToken);
    Task SetSolarChargerUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken);
}