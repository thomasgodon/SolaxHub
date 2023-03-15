namespace SolaxHub.Dsmr;

public interface ISolaxClient
{
    Task Start(CancellationToken cancellationToken);
}