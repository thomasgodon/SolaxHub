using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.PowerControl;

internal sealed class PowerControlStateService : IPowerControlStateService
{
    private int _maxGridImportWatts;
    private int _activeMode;

    public int MaxGridImportWatts => _maxGridImportWatts;

    public void SetMaxGridImportWatts(int watts)
    {
        Interlocked.Exchange(ref _maxGridImportWatts, watts);
    }

    public PowerControlMode ActiveMode => (PowerControlMode)_activeMode;

    public void SetActiveMode(PowerControlMode mode)
    {
        Interlocked.Exchange(ref _activeMode, (int)mode);
    }
}
