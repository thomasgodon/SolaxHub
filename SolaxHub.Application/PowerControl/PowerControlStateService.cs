using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.PowerControl;

internal sealed class PowerControlStateService : IPowerControlStateService
{
    private int _activeMode;
    private int _powerTargetWatts;

    public PowerControlMode ActiveMode => (PowerControlMode)_activeMode;

    public void SetActiveMode(PowerControlMode mode)
    {
        Interlocked.Exchange(ref _activeMode, (int)mode);
    }

    public int PowerTargetWatts => _powerTargetWatts;

    public void SetPowerTarget(int watts)
    {
        Interlocked.Exchange(ref _powerTargetWatts, watts);
    }
}
