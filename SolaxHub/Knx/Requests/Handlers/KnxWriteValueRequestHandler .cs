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
        private readonly ILogger<KnxWriteValueRequestHandler> _logger;
        private readonly ISolaxControllerService _solaxControllerService;
        private readonly Dictionary<GroupAddress, string> _writeGroupAddressCapabilityMapping;

        public KnxWriteValueRequestHandler(
            IOptions<KnxOptions> options,
            ILogger<KnxWriteValueRequestHandler> logger,
            ISolaxControllerService solaxControllerService)
        {
            _logger = logger;
            _solaxControllerService = solaxControllerService;
            _writeGroupAddressCapabilityMapping = BuildWriteGroupAddressCapabilityMapping(options.Value);
        }

        public async Task Handle(KnxWriteValueRequest request, CancellationToken cancellationToken)
        {
            if (_writeGroupAddressCapabilityMapping.TryGetValue(request.GroupAddress, out var capability) is false)
            {
                return;
            }
            
            await ProcessCapabilityValueAsync(capability, request.Value, cancellationToken);
        }

        private static Dictionary<GroupAddress, string> BuildWriteGroupAddressCapabilityMapping(KnxOptions options)
            => options.GetWriteGroupAddressesFromOptions()
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);

        private async Task ProcessCapabilityValueAsync(string capability, byte[] value, CancellationToken cancellationToken)
        {
            switch (capability)
            {
                case "InverterUseMode": 
                    await _solaxControllerService.SetInverterUseModeAsync((SolaxInverterUseMode)BitConverter.ToInt16(value), cancellationToken);
                    break;

                default:
                    _logger.LogWarning("Writing parameter '{parameter}' not implemented", capability);
                    break;
            }
        }
    }
}
