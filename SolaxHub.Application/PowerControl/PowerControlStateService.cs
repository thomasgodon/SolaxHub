namespace SolaxHub.Application.PowerControl;

internal sealed class PowerControlStateService : IPowerControlStateService
{
    private int _maxGridImportWatts;

    public int MaxGridImportWatts => _maxGridImportWatts;

    public void SetMaxGridImportWatts(int watts)
    {
        Interlocked.Exchange(ref _maxGridImportWatts, watts);
    }
}
