using MediatR;
using Microsoft.Extensions.Logging;
using SolaxHub.Application.Inverter.Commands.SetPowerControl;
using SolaxHub.Application.Inverter.Services;
using SolaxHub.Application.PowerControl;
using SolaxHub.Domain.Inverter;

namespace SolaxHub.Application.Inverter.Commands.SetBatteryChargePowerTarget;

internal sealed class SetBatteryChargePowerTargetCommandHandler : IRequestHandler<SetBatteryChargePowerTargetCommand>
{
    private readonly ISender _sender;
    private readonly IInverterStateService _stateService;
    private readonly IPowerControlStateService _powerControlState;
    private readonly ILogger<SetBatteryChargePowerTargetCommandHandler> _logger;

    public SetBatteryChargePowerTargetCommandHandler(
        ISender sender,
        IInverterStateService stateService,
        IPowerControlStateService powerControlState,
        ILogger<SetBatteryChargePowerTargetCommandHandler> logger)
    {
        _sender = sender;
        _stateService = stateService;
        _powerControlState = powerControlState;
        _logger = logger;
    }

    public async Task Handle(SetBatteryChargePowerTargetCommand request, CancellationToken cancellationToken)
    {
        if (request.Watts <= 0)
        {
            await _sender.Send(new SetPowerControlCommand(PowerControlMode.Disabled, 0), cancellationToken);
            return;
        }

        var inverter = _stateService.Inverter;

        // effectiveCharge = min(Watts, MaxGridImport + PV − HouseLoad)
        // This keeps total grid import ≤ MaxGridImportWatts by reducing battery charging
        // when house load consumes grid headroom.
        var gridHeadroom = _powerControlState.MaxGridImportWatts + (int)inverter.Solar.Power1 - inverter.HouseLoad;
        var effectiveCharge = Math.Max(0, Math.Min(request.Watts, gridHeadroom));

        _logger.LogDebug(
            "Charge: requested={Requested}W houseLoad={HouseLoad}W pv={Pv}W maxGrid={MaxGrid}W effective={Effective}W",
            request.Watts, inverter.HouseLoad, inverter.Solar.Power1, _powerControlState.MaxGridImportWatts, effectiveCharge);

        if (effectiveCharge == 0)
        {
            await _sender.Send(new SetPowerControlCommand(PowerControlMode.Disabled, 0), cancellationToken);
            return;
        }

        // Mode 1 (PowerControlMode): GridWTarget = grid needed for house + grid needed for battery charge.
        // Grid covers house load that PV can't supply, plus battery charge that PV surplus can't cover.
        var pvSurplus = Math.Max(0, (int)inverter.Solar.Power1 - inverter.HouseLoad);
        var gridForBattery = Math.Max(0, effectiveCharge - pvSurplus);
        var gridForHouse = Math.Max(0, inverter.HouseLoad - (int)inverter.Solar.Power1);
        var gridWTarget = gridForHouse + gridForBattery;

        _logger.LogDebug(
            "Charge: pvSurplus={PvSurplus}W gridForBattery={GridForBattery}W gridForHouse={GridForHouse}W gridWTarget={GridWTarget}W",
            pvSurplus, gridForBattery, gridForHouse, gridWTarget);

        await _sender.Send(new SetPowerControlCommand(PowerControlMode.PowerControlMode, gridWTarget), cancellationToken);
    }
}
