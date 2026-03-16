using Knx.Falcon;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolaxHub.Application.Inverter.Commands.SetInverterUseMode;
using SolaxHub.Application.Inverter.Commands.SetPowerControl;
using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Knx.Extensions;
using SolaxHub.Infrastructure.Knx.Options;
using SolaxHub.Infrastructure.Knx.Services;

namespace SolaxHub.Infrastructure.Knx.Requests.Handlers;

internal class KnxWriteValueRequestHandler : IRequestHandler<KnxWriteValueRequest>
{
    private readonly ILogger<KnxWriteValueRequestHandler> _logger;
    private readonly ISender _sender;
    private readonly IPowerControlBufferService _powerControlBuffer;
    private readonly Dictionary<GroupAddress, string> _writeGroupAddressCapabilityMapping;

    public KnxWriteValueRequestHandler(
        IOptions<KnxOptions> options,
        ILogger<KnxWriteValueRequestHandler> logger,
        ISender sender,
        IPowerControlBufferService powerControlBuffer)
    {
        _logger = logger;
        _sender = sender;
        _powerControlBuffer = powerControlBuffer;
        _writeGroupAddressCapabilityMapping = options.Value.GetWriteGroupAddressesFromOptions()
            .ToDictionary(
                m => GroupAddress.Parse(m.Value),
                m => m.Key);
    }

    public async Task Handle(KnxWriteValueRequest request, CancellationToken cancellationToken)
    {
        if (!_writeGroupAddressCapabilityMapping.TryGetValue(request.GroupAddress, out var capability))
            return;

        switch (capability)
        {
            case "InverterUseMode":
                var useMode = (InverterUseMode)request.Value[0];
                _logger.LogInformation("Setting inverter use mode to {Mode}", useMode);
                await _sender.Send(new SetInverterUseModeCommand(useMode), cancellationToken);
                break;

            case "PowerControlMode":
                var mode = (PowerControlMode)request.Value[0];
                var data = _powerControlBuffer.BuildRegisterBlock(mode);
                _logger.LogInformation("Setting power control mode to {Mode}", mode);
                await _sender.Send(new SetPowerControlCommand(mode, data), cancellationToken);
                break;

            case "PowerControlActivePower":
                var activePower = (int)BitConverter.ToSingle(request.Value);
                _logger.LogInformation("Buffering power control active power: {Watts}W", activePower);
                _powerControlBuffer.SetActivePower(activePower);
                break;

            case "PowerControlDuration":
                var duration = BitConverter.ToUInt16(request.Value);
                _logger.LogInformation("Buffering power control duration: {Seconds}s", duration);
                _powerControlBuffer.SetDuration(duration);
                break;

            case "PowerControlTargetSoc":
                var targetSoc = request.Value[0];
                _logger.LogInformation("Buffering power control target SOC: {Percent}%", targetSoc);
                _powerControlBuffer.SetTargetSoc(targetSoc);
                break;

            case "PowerControlChargeDischargePower":
                var chargeDischargePower = (int)BitConverter.ToSingle(request.Value);
                _logger.LogInformation("Buffering power control charge/discharge power: {Watts}W", chargeDischargePower);
                _powerControlBuffer.SetChargeDischargePower(chargeDischargePower);
                break;

            case "BatteryDischargePowerTarget":
                var dischargePowerTarget = (int)BitConverter.ToSingle(request.Value);
                if (dischargePowerTarget <= 0)
                {
                    _logger.LogInformation("Disabling battery discharge power target");
                    var disableData = _powerControlBuffer.BuildRegisterBlock(PowerControlMode.Disabled);
                    await _sender.Send(new SetPowerControlCommand(PowerControlMode.Disabled, disableData), cancellationToken);
                }
                else
                {
                    _logger.LogInformation("Setting battery discharge power target to {Watts}W", dischargePowerTarget);
                    _powerControlBuffer.SetChargeDischargePower(dischargePowerTarget);
                    var selfConsumeData = _powerControlBuffer.BuildRegisterBlock(PowerControlMode.SelfConsumeChargeDischargeMode);
                    await _sender.Send(new SetPowerControlCommand(PowerControlMode.SelfConsumeChargeDischargeMode, selfConsumeData), cancellationToken);
                }
                break;

            default:
                _logger.LogWarning("Writing parameter '{Parameter}' not implemented", capability);
                break;
        }
    }
}
