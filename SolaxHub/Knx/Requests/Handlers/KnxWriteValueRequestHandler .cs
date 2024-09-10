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
            if (!_writeGroupAddressCapabilityMapping.TryGetValue(request.GroupAddress, out var capability))
            {
                return;
            }

            try
            {
                await ProcessCapabilityValue(capability, request.Value);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{Message}", e.Message);
            }
        }

        private static Dictionary<GroupAddress, string> BuildWriteGroupAddressCapabilityMapping(KnxOptions options)
            => options.GetWriteGroupAddressesFromOptions()
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);

        private Task ProcessCapabilityValue(string capability, byte[] value)
        {
            switch (capability)
            {
                case "RemoteControlMode": 
                    _solaxControllerService.PowerControlMode = (SolaxPowerControlMode)value[0];
                    break;

                case "ImportLimit":
                    _solaxControllerService.PowerControlImportLimit = BitConverter.ToSingle(value.Reverse().ToArray());
                    break;

                case "BatteryChargeLimit":
                    _solaxControllerService.PowerControlBatteryChargeLimit = BitConverter.ToSingle(value.Reverse().ToArray());
                    break;

                default:
                    _logger.LogWarning("Writing parameter '{Parameter}' not implemented", capability);
                    break;
            }

            return Task.CompletedTask;
        }
    }
}
