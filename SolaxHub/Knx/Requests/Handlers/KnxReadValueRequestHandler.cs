using Knx.Falcon;
using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Extensions;
using SolaxHub.Knx.Models;

namespace SolaxHub.Knx.Requests.Handlers
{
    internal class KnxReadValueRequestHandler : IRequestHandler<KnxReadValueRequest, KnxValue?>
    {
        private readonly Dictionary<GroupAddress, string> _readGroupAddressCapabilityMapping;

        public KnxReadValueRequestHandler(IOptions<KnxOptions> options)
        {
            _readGroupAddressCapabilityMapping = BuildReadGroupAddressCapabilityMapping(options.Value);
        }

        public Task<KnxValue?> Handle(KnxReadValueRequest request, CancellationToken cancellationToken)
        {
            return Task.FromResult<KnxValue?>(null);
        }

        private static Dictionary<GroupAddress, string> BuildReadGroupAddressCapabilityMapping(KnxOptions options)
            => options.GetReadGroupAddressesFromOptions()
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);
    }
}
