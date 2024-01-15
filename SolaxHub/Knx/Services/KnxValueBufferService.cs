using Knx.Falcon;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Services
{
    internal class KnxValueBufferService : IKnxValueBufferService
    {
        private readonly Dictionary<string, KnxValue> _capabilityKnxValueMapping;
        private readonly Dictionary<GroupAddress, string> _readGroupAddressCapabilityMapping;

        public KnxValueBufferService(IOptions<KnxOptions> options)
        {
            _capabilityKnxValueMapping = BuildCapabilityKnxValueMapping(options.Value);
            _readGroupAddressCapabilityMapping = BuildReadGroupAddressCapabilityMapping(options.Value);
        }

        public KnxValue? UpdateValue(string capability, byte[] value)
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

        private static Dictionary<string, KnxValue> BuildCapabilityKnxValueMapping(KnxOptions knxOptions)
        {
            var solaxData = new Dictionary<string, KnxValue>(knxOptions.ReadGroupAddresses.Count);

            foreach (var groupAddressMapping in GetReadGroupAddressesFromOptions(knxOptions))
            {
                solaxData.Add(groupAddressMapping.Key, new KnxValue(groupAddressMapping.Value));
            }

            return solaxData;
        }

        private static Dictionary<GroupAddress, string> BuildReadGroupAddressCapabilityMapping(KnxOptions knxOptions)
            => GetReadGroupAddressesFromOptions(knxOptions)
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);

        private static IEnumerable<KeyValuePair<string, string>> GetReadGroupAddressesFromOptions(KnxOptions options)
            => options.ReadGroupAddresses
                .Where(
                    mapping => string.IsNullOrEmpty(mapping.Value) is false);
    }
}
