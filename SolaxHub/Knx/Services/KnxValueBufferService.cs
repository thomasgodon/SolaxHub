using Microsoft.Extensions.Options;
using SolaxHub.Knx.Extensions;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Services
{
    internal class KnxValueBufferService : IKnxValueBufferService
    {
        private readonly Dictionary<string, KnxValue> _capabilityKnxValueMapping;
        private readonly object _mappingLock = new();

        public KnxValueBufferService(IOptions<KnxOptions> options)
        {
            _capabilityKnxValueMapping = BuildCapabilityKnxValueMapping(options.Value);
        }

        public KnxValue? UpdateValue(string capability, byte[] value)
        {
            lock (_mappingLock)
            {
                if (_capabilityKnxValueMapping.TryGetValue(capability, out var knxSolaxValue) is false)
                {
                    return null;
                }

                if (knxSolaxValue.Value is not null)
                {
                    if (knxSolaxValue.Value.SequenceEqual(value))
                    {
                        return null;
                    }
                }

                _capabilityKnxValueMapping[capability].Value = value;
                return _capabilityKnxValueMapping[capability];
            }
        }

        public IReadOnlyDictionary<string, KnxValue> GetKnxValues()
        {
            lock (_mappingLock)
            {
                return _capabilityKnxValueMapping.ToDictionary(key => key.Key, value => value.Value);
            }
        }

        private static Dictionary<string, KnxValue> BuildCapabilityKnxValueMapping(KnxOptions options)
        {
            var solaxData = new Dictionary<string, KnxValue>(options.ReadGroupAddresses.Count);

            foreach (var groupAddressMapping in options.GetReadGroupAddressesFromOptions())
            {
                solaxData.Add(groupAddressMapping.Key, new KnxValue(groupAddressMapping.Value));
            }

            return solaxData;
        }
    }
}
