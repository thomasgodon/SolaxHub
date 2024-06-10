using SolaxHub.Knx.Models;
using SolaxHub.Solax.Models;

namespace SolaxHub.Knx.Services
{
    internal interface IKnxValueBufferService
    {
        IEnumerable<KnxValue> UpdateKnxValues(SolaxData data);
        IReadOnlyDictionary<string, KnxValue> GetKnxValues();
    }
}
