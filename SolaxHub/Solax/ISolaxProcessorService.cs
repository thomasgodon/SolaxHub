using Newtonsoft.Json.Linq;

namespace SolaxHub.Solax;

internal interface ISolaxProcessorService
{
    Task ProcessJson(JObject json, CancellationToken cancellationToken);
}