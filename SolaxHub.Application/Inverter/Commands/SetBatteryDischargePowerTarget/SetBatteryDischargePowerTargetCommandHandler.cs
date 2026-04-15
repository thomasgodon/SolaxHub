using MediatR;
using SolaxHub.Application.Inverter.Commands.SetPowerControl;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetBatteryDischargePowerTarget;

internal sealed class SetBatteryDischargePowerTargetCommandHandler : IRequestHandler<SetBatteryDischargePowerTargetCommand>
{
    private readonly ISender _sender;

    public SetBatteryDischargePowerTargetCommandHandler(ISender sender)
    {
        _sender = sender;
    }

    public async Task Handle(SetBatteryDischargePowerTargetCommand request, CancellationToken cancellationToken)
    {
        if (request.Watts <= 0)
            await _sender.Send(new SetPowerControlCommand(PowerControlMode.Disabled, 0), cancellationToken);
        else
            await _sender.Send(new SetPowerControlCommand(PowerControlMode.PowerControlMode, -request.Watts), cancellationToken);
    }
}
