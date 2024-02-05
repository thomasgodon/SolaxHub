using Knx.Falcon;
using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Extensions;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Requests.Handlers
{
    internal class KnxInitialReadValueRequestHandler : IRequestHandler<KnxInitialReadValueRequest, IReadOnlyList<KnxValue>>
    {
        private readonly Dictionary<GroupAddress, string> _writeGroupAddressCapabilityMapping;

        public KnxInitialReadValueRequestHandler(
            IOptions<KnxOptions> options)
        {
            _writeGroupAddressCapabilityMapping = BuildWriteGroupAddressCapabilityMapping(options.Value);
        }

        public Task<IReadOnlyList<KnxValue>> Handle(KnxInitialReadValueRequest request, CancellationToken cancellationToken)
            => Task.FromResult<IReadOnlyList<KnxValue>>(KnxWriteAddresses().ToList());

        private IEnumerable<KnxValue> KnxWriteAddresses()
            => _writeGroupAddressCapabilityMapping.Select(mapping => new KnxValue(mapping.Key));

        private static Dictionary<GroupAddress, string> BuildWriteGroupAddressCapabilityMapping(KnxOptions options)
            => options.GetWriteGroupAddressesFromOptions()
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);
    }
}
