using SolaxHub.Solax.Models;

namespace SolaxHub.Solax;

internal interface ISolaxProcessorService
{
    [Obsolete]
    Task ProcessData(SolaxData data, CancellationToken cancellationToken);

    SolaxData ReadSolaxData();
}