using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Knx.Models;

namespace SolaxHub.Infrastructure.Knx.Services;

internal interface IKnxValueBufferService
{
    IEnumerable<KnxValue> UpdateKnxValues(Inverter inverter);
    IReadOnlyDictionary<string, KnxValue> GetKnxValues();
    KnxValue? UpdateMaxGridImportWatts(int watts);
}
