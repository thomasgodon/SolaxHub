using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Services
{
    internal interface IKnxValueBufferService
    {
        KnxValue? UpdateValue(string capability, byte[] value);
        IReadOnlyDictionary<string, KnxValue> GetKnxValues();
    }
}
