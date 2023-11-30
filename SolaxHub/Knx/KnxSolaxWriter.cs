using Knx.Falcon;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Client;
using SolaxHub.Solax;

namespace SolaxHub.Knx
{
    internal class KnxSolaxWriter : ISolaxWriter, IKnxWriteDelegate
    {
        private readonly IKnxClient _knxClient;
        private readonly Dictionary<GroupAddress, string> _capabilityAddressMapping;
        private ISolaxClient? _solaxClient;

        public KnxSolaxWriter(IKnxClient knxClient, IOptions<KnxOptions> options)
        {
            _knxClient = knxClient;
            _knxClient.SetWriteDelegate(this);
            _capabilityAddressMapping = BuildCapabilityWriteAddressMapping(options.Value);
        }

        public void SetSolaxClient(ISolaxClient solaxClient)
        {
            _solaxClient = solaxClient;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _knxClient.ConnectAsync(cancellationToken);
        }

        public async Task ProcessWriteAsync(GroupAddress groupsAddress, byte[] value, CancellationToken cancellationToken)
        {
            if (_solaxClient is null)
            {
                return;
            }

            if (_capabilityAddressMapping.TryGetValue(groupsAddress, out var capability) is false)
            {
                return;
            }

            await WriteModbusAsync(value, capability, _solaxClient, cancellationToken);
        }

        public static async Task WriteModbusAsync(byte[] value, string capability, ISolaxClient solaxClient, CancellationToken cancellationToken)
        {
            switch (capability)
            {
                case nameof(SolaxData.InverterUseMode):
                    await solaxClient.SetSolarChargerUseModeAsync((SolaxInverterUseMode)BitConverter.ToUInt16(value), cancellationToken);
                    break;
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetWriteGroupAddressesFromOptions(KnxOptions options)
            => options.WriteGroupAddresses
                .Where(
                    mapping => string.IsNullOrEmpty(mapping.Value) is false);

        private static Dictionary<GroupAddress, string> BuildCapabilityWriteAddressMapping(KnxOptions knxOptions)
            => GetWriteGroupAddressesFromOptions(knxOptions)
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);
    }
}
