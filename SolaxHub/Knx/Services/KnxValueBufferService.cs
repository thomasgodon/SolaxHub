using Knx.Falcon;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Services
{
    internal class KnxValueBufferService : IKnxValueBufferService
    {
        private readonly Dictionary<string, KnxValue> _knxValueBuffer;
        private readonly Dictionary<GroupAddress, string> _capabilityAddressMapping;

        public KnxValueBufferService(IOptions<KnxOptions> options)
        {
            _knxValueBuffer = BuildKnxValueBuffer(options.Value);
            _capabilityAddressMapping = BuildCapabilityReadAddressMapping(options.Value);
        }

        private static Dictionary<string, KnxValue> BuildKnxValueBuffer(KnxOptions knxOptions)
        {
            var solaxData = new Dictionary<string, KnxValue>(knxOptions.ReadGroupAddresses.Count);

            foreach (var groupAddressMapping in GetReadGroupAddressesFromOptions(knxOptions))
            {
                solaxData.Add(groupAddressMapping.Key, new KnxValue(groupAddressMapping.Value));
            }

            return solaxData;
        }

        public KnxValue? UpdateValue(string capability, byte[] value)
        {
            if (_knxValueBuffer.TryGetValue(capability, out var knxSolaxValue) is false)
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

            _knxValueBuffer[capability].Value = value;
            return _knxValueBuffer[capability];
        }

        private static IEnumerable<KeyValuePair<string, string>> GetReadGroupAddressesFromOptions(KnxOptions options)
            => options.ReadGroupAddresses
                .Where(
                    mapping => string.IsNullOrEmpty(mapping.Value) is false);

        private static Dictionary<GroupAddress, string> BuildCapabilityReadAddressMapping(KnxOptions knxOptions)
            => GetReadGroupAddressesFromOptions(knxOptions)
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);
    }
}
