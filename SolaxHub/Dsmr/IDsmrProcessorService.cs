namespace SolaxHub.Dsmr;

internal interface IDsmrProcessorService
{
    Task ProcessMessage(string message, CancellationToken cancellationToken);
}