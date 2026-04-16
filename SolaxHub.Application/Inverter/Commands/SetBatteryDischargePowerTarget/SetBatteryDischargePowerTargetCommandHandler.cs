using MediatR;
using Microsoft.Extensions.Logging;
using SolaxHub.Application.Inverter.Commands.SetPowerControl;
using SolaxHub.Application.Inverter.Services;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetBatteryDischargePowerTarget;

internal sealed class SetBatteryDischargePowerTargetCommandHandler : IRequestHandler<SetBatteryDischargePowerTargetCommand>
{
    private readonly ISender _sender;
    private readonly IInverterStateService _stateService;
    private readonly ILogger<SetBatteryDischargePowerTargetCommandHandler> _logger;

    public SetBatteryDischargePowerTargetCommandHandler(
        ISender sender,
        IInverterStateService stateService,
        ILogger<SetBatteryDischargePowerTargetCommandHandler> logger)
    {
        _sender = sender;
        _stateService = stateService;
        _logger = logger;
    }

    public async Task Handle(SetBatteryDischargePowerTargetCommand request, CancellationToken cancellationToken)
    {
        if (request.Watts <= 0)
        {
            await _sender.Send(new SetPowerControlCommand(PowerControlMode.Disabled, 0), cancellationToken);
            return;
        }

        var inverter = _stateService.Inverter;
        var neededFromBattery = Math.Max(0, inverter.HouseLoad - (int)inverter.Solar.Power1);
        var effectiveWatts = Math.Min(request.Watts, neededFromBattery);

        _logger.LogDebug(
            "Discharge: requested={Requested}W houseLoad={HouseLoad}W pv={Pv}W effective={Effective}W",
            request.Watts, inverter.HouseLoad, inverter.Solar.Power1, effectiveWatts);

        if (effectiveWatts == 0)
        {
            await _sender.Send(new SetPowerControlCommand(PowerControlMode.Disabled, 0), cancellationToken);
            return;
        }

        // Mode 1 (PowerControlMode): GridWTarget positive = inverter imports from grid.
        // Setting it to (neededFromBattery − effectiveWatts) makes the inverter import exactly
        // what the battery can't cover, so battery discharges at effectiveWatts.
        var gridWTarget = neededFromBattery - effectiveWatts;
        await _sender.Send(new SetPowerControlCommand(PowerControlMode.PowerControlMode, gridWTarget), cancellationToken);
    }
}
