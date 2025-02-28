namespace SolaxHub.Solax.Services;

public interface ISolaxPollingService
{
    Task ProcessAsync(CancellationToken cancellationToken);
}