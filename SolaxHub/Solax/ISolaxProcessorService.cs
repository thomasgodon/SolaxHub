namespace SolaxHub.Solax;

internal interface ISolaxProcessorService
{
    Task ProcessMessage(string message, CancellationToken cancellationToken);
}