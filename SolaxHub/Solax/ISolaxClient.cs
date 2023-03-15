namespace SolaxHub.Solax;

public interface ISolaxClient
{
    Task Start(CancellationToken cancellationToken);
}