using SolaxHub.Solax.Models;

namespace SolaxHub.Solax
{
    internal interface ISolaxProcessor
    {
        Task ProcessData(SolaxData data, CancellationToken cancellationToken);
    }
}
