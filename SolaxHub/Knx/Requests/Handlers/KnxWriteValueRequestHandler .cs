using Knx.Falcon;
using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Extensions;
using SolaxHub.Knx.Models;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Services;

namespace SolaxHub.Knx.Requests.Handlers
{
    internal class KnxWriteValueRequestHandler : IRequestHandler<KnxWriteValueRequest>
    {
        private readonly ISolaxControllerService _solaxControllerService;
        private readonly Dictionary<GroupAddress, string> _writeGroupAddressCapabilityMapping;

        public KnxWriteValueRequestHandler(
            IOptions<KnxOptions> options,
            ISolaxControllerService solaxControllerService)
        {
            _solaxControllerService = solaxControllerService;
            _writeGroupAddressCapabilityMapping = BuildWriteGroupAddressCapabilityMapping(options.Value);
        }

        public Task Handle(KnxWriteValueRequest request, CancellationToken cancellationToken)
        {
            return _writeGroupAddressCapabilityMapping.TryGetValue(request.GroupAddress, out var capability) is false 
                ? Task.CompletedTask 
                : ProcessCapabilityValue(capability, request.Value);
        }

        private static Dictionary<GroupAddress, string> BuildWriteGroupAddressCapabilityMapping(KnxOptions options)
            => options.GetWriteGroupAddressesFromOptions()
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);

        private Task ProcessCapabilityValue(string capability, byte[] value)
            => capability switch
            {
                "InverterUseMode" => _solaxControllerService.SetInverterUseMode((SolaxInverterUseMode)BitConverter.ToInt16(value)),
                _ => Task.CompletedTask
            };
    }
}
