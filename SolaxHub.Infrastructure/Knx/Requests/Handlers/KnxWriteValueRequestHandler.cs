using Knx.Falcon;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SolaxHub.Application.Inverter.Commands.SetInverterUseMode;
using SolaxHub.Application.Inverter.Commands.SetPowerControl;
using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Knx.Extensions;
using SolaxHub.Infrastructure.Knx.Options;

namespace SolaxHub.Infrastructure.Knx.Requests.Handlers;

internal class KnxWriteValueRequestHandler : IRequestHandler<KnxWriteValueRequest>
{
    private const ushort DefaultDuration = 20;

    private readonly ILogger<KnxWriteValueRequestHandler> _logger;
    private readonly ISender _sender;
    private readonly Dictionary<GroupAddress, string> _writeGroupAddressCapabilityMapping;

    public KnxWriteValueRequestHandler(
        IOptions<KnxOptions> options,
        ILogger<KnxWriteValueRequestHandler> logger,
        ISender sender)
    {
        _logger = logger;
        _sender = sender;
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

            case "BatteryDischargePowerTarget":
                var dischargePowerTarget = (int)BitConverter.ToSingle(request.Value);
                if (dischargePowerTarget <= 0)
                {
                    _logger.LogInformation("Disabling battery discharge power target");
                    var disableData = BuildRegisterBlock(PowerControlMode.Disabled, 0);
                    await _sender.Send(new SetPowerControlCommand(PowerControlMode.Disabled, disableData), cancellationToken);
                }
                else
                {
                    _logger.LogInformation("Setting battery discharge power target to {Watts}W", dischargePowerTarget);
                    var selfConsumeData = BuildRegisterBlock(PowerControlMode.SelfConsumeChargeDischargeMode, dischargePowerTarget);
                    await _sender.Send(new SetPowerControlCommand(PowerControlMode.SelfConsumeChargeDischargeMode, selfConsumeData), cancellationToken);
                }
                break;

            default:
                _logger.LogWarning("Writing parameter '{Parameter}' not implemented", capability);
                break;
        }
    }

    private static byte[] BuildRegisterBlock(PowerControlMode mode, int chargeDischargePower)
    {
        var data = new byte[30];

        // 0x7C: Power Control Trigger (U16) - mode value
        WriteU16BigEndian(data, 0, (ushort)mode);

        // 0x7D: Reserved (U16) - 0
        // 0x7E-0x7F: Active Power (S32) - 0
        // 0x80-0x81: Reactive Power (S32) - 0

        // 0x82: Duration (U16)
        WriteU16BigEndian(data, 12, DefaultDuration);

        // 0x83: Target SOC (U16) - 0
        // 0x84-0x85: Target Energy (S32) - 0

        // 0x86-0x87: Charge/Discharge Power (S32, swapped word order)
        WriteS32SwappedWordOrder(data, 20, chargeDischargePower);

        // 0x88: Timeout (U16) - 0
        // 0x89-0x8A: Push Mode Power (S32) - 0

        return data;
    }

    private static void WriteU16BigEndian(byte[] buffer, int offset, ushort value)
    {
        buffer[offset] = (byte)(value >> 8);
        buffer[offset + 1] = (byte)(value & 0xFF);
    }

    private static void WriteS32SwappedWordOrder(byte[] buffer, int offset, int value)
    {
        // Low word first, then high word (each word is big-endian)
        buffer[offset] = (byte)(value >> 8);
        buffer[offset + 1] = (byte)(value & 0xFF);
        buffer[offset + 2] = (byte)(value >> 24);
        buffer[offset + 3] = (byte)(value >> 16);
    }
}
