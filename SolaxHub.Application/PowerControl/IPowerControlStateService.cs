using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.PowerControl;

public interface IPowerControlStateService
{
    PowerControlMode ActiveMode { get; }
    void SetActiveMode(PowerControlMode mode);

    int PowerTargetWatts { get; }
    void SetPowerTarget(int watts);
}
