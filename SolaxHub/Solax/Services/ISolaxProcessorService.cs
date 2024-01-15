using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Services;

internal interface ISolaxProcessorService
{
    Task ConsumeSolaxDataAsync(SolaxData data, CancellationToken cancellationToken);
    SolaxData? ReadSolaxData();
}