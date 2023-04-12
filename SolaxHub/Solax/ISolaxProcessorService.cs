using Newtonsoft.Json.Linq;
using SolaxHub.Solax.Http;

namespace SolaxHub.Solax;

internal interface ISolaxProcessorService
{
    Task ProcessData(SolaxData data, CancellationToken cancellationToken);
}