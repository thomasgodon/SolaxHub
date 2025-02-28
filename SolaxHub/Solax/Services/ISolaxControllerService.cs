namespace SolaxHub.Solax.Services;

public interface ISolaxControllerService
{
    Task ProcessAsync(CancellationToken cancellationToken);
}