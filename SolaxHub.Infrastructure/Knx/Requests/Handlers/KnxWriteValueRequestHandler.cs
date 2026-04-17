using Knx.Falcon;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolaxHub.Application.Inverter.Commands.SetInverterUseMode;
using SolaxHub.Application.Inverter.Commands.SetPowerControl;

using SolaxHub.Application.Inverter.Services;
using SolaxHub.Application.PowerControl;
using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Knx.Extensions;
using SolaxHub.Infrastructure.Knx.Options;

namespace SolaxHub.Infrastructure.Knx.Requests.Handlers;

internal class KnxWriteValueRequestHandler : IRequestHandler<KnxWriteValueRequest>
{
    private readonly ILogger<KnxWriteValueRequestHandler> _logger;
    private readonly ISender _sender;
    private readonly IInverterCommandQueue _commandQueue;
    private readonly IPowerControlStateService _powerControlState;
    private readonly Dictionary<GroupAddress, string> _writeGroupAddressCapabilityMapping;

    public KnxWriteValueRequestHandler(
        IOptions<KnxOptions> options,
        ILogger<KnxWriteValueRequestHandler> logger,
        ISender sender,
        IInverterCommandQueue commandQueue,
        IPowerControlStateService powerControlState)
    {
        _logger = logger;
        _sender = sender;
        _commandQueue = commandQueue;
        _powerControlState = powerControlState;
        _writeGroupAddressCapabilityMapping = options.Value.GetWriteGroupAddressesFromOptions()
            .ToDictionary(
                m => GroupAddress.Parse(m.Value),
                m => m.Key);
    }

    public Task Handle(KnxWriteValueRequest request, CancellationToken cancellationToken)
    {
        if (!_writeGroupAddressCapabilityMapping.TryGetValue(request.GroupAddress, out var capability))
            return Task.CompletedTask;

        switch (capability)
        {
            case "InverterUseMode":
                var useMode = (InverterUseMode)request.Value[0];
                _logger.LogInformation("Setting inverter use mode to {Mode}", useMode);
                _commandQueue.Enqueue(ct => _sender.Send(new SetInverterUseModeCommand(useMode), ct));
                break;

            case "PowerControlMode":
                var mode = (PowerControlMode)request.Value[0];
                _logger.LogInformation("Setting power control mode to {Mode}", mode);
                _powerControlState.SetActiveMode(mode);
                if (mode == PowerControlMode.Disabled)
                    _commandQueue.Enqueue(ct => _sender.Send(new SetPowerControlCommand(PowerControlMode.Disabled, 0), ct));
                break;

            case "PowerControlPowerTarget":
                var powerWatts = (int)BitConverter.ToSingle(request.Value);
                _logger.LogInformation("Setting power control target to {Watts}W", powerWatts);
                _powerControlState.SetPowerTarget(powerWatts);
                break;

            default:
                _logger.LogWarning("Writing parameter '{Parameter}' not implemented", capability);
                break;
        }

        return Task.CompletedTask;
    }
}
