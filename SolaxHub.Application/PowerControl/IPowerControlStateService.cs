namespace SolaxHub.Application.PowerControl;

public interface IPowerControlStateService
{
    int MaxGridImportWatts { get; }
    void SetMaxGridImportWatts(int watts);
}
