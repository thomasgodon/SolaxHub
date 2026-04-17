using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.PowerControl;

public interface IPowerControlStateService
{
    int MaxGridImportWatts { get; }
    void SetMaxGridImportWatts(int watts);

    PowerControlMode ActiveMode { get; }
    void SetActiveMode(PowerControlMode mode);
}
