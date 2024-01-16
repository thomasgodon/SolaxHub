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

            try
            {
                await ProcessCapabilityValue(capability, request.Value);
            }
            catch (Exception e)
            {
                _logger.LogError("{message}", e.Message);
            }
        }

        private static Dictionary<GroupAddress, string> BuildWriteGroupAddressCapabilityMapping(KnxOptions options)
            => options.GetWriteGroupAddressesFromOptions()
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);

        private async Task ProcessCapabilityValue(string capability, byte[] value)
        {
            switch (capability)
            {
                case "RemoteControlMode": 
                    await _solaxControllerService.SetRemoteControlPowerControlModeAsync((SolaxRemoteControlPowerControlMode)value[0]);
                    break;

                default:
                    _logger.LogWarning("Writing parameter '{parameter}' not implemented", capability);
                    break;
            }
        }
    }
}
