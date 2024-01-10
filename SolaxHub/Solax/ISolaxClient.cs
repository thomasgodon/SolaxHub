namespace SolaxHub.Solax;

public interface ISolaxClient
{
    Task Start(CancellationToken cancellationToken);
    Task SetSolarChargerUseModeAsync(SolaxInverterUseMode useMode, CancellationToken cancellationToken);
}