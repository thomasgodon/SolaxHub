using MediatR;
using SolaxHub.Solax.Commands;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Queries;

namespace SolaxHub.Solax.Services;

internal class SolaxPollingService : ISolaxPollingService
{
    private readonly ISender _sender;
    private readonly IPublisher _publisher;

    public SolaxPollingService(ISender sender,
        IPublisher publisher)
    {
        _sender = sender;
        _publisher = publisher;
    }

    public async Task ProcessAsync(CancellationToken cancellationToken)
    {
        SolaxLockState currentLockState = await _sender.Send(new GetLockStateQuery(), cancellationToken);
        if (currentLockState != SolaxLockState.UnlockedAdvanced)
        {
            await _sender.Send(new SetLockStateCommand(SolaxLockState.UnlockedAdvanced), cancellationToken);
        }

        SolaxData data = new()
        {
            LockState = await _sender.Send(new GetLockStateQuery(), cancellationToken),
            BatteryCapacity = await _sender.Send(new GetBatteryCapacityQuery(), cancellationToken),
            BatteryInputEnergyToday = await _sender.Send(new GetTodayBatteryInputEnergyQuery(), cancellationToken),
            BatteryInputEnergyTotal = await _sender.Send(new GetTotalBatteryInputEnergyQuery(), cancellationToken),
            BatteryOutputEnergyToday = await _sender.Send(new GetTodayBatteryOutputEnergyQuery(), cancellationToken),
            BatteryOutputEnergyTotal = await _sender.Send(new GetTotalBatteryOutputEnergyQuery(), cancellationToken),
            InverterSerialNumber = await _sender.Send(new GetSerialNumberQuery(), cancellationToken),
            RegistrationCodePocket = await _sender.Send(new GetRegistrationCodePocketQuery(), cancellationToken),
            BatteryPower = await _sender.Send(new GetBatteryPowerQuery(), cancellationToken),
            InverterVoltage = await _sender.Send(new GetInverterVoltageQuery(), cancellationToken),
            InverterPower = await _sender.Send(new GetInverterPowerQuery(), cancellationToken),
            FeedInPower = await _sender.Send(new GetFeedInPowerQuery(), cancellationToken),
            ConsumeEnergy = await _sender.Send(new GetConsumeEnergyQuery(), cancellationToken),
            FeedInEnergy = await _sender.Send(new GetFeedInEnergyQuery(), cancellationToken),
            InverterStatus = await _sender.Send(new GetInverterStatusQuery(), cancellationToken),
            PvPower1 = await _sender.Send(new GetPvCurrent1Query(), cancellationToken),
            PvVolt1 = await _sender.Send(new GetPvVolt1Query(), cancellationToken),
            PvCurrent1 = await _sender.Send(new GetPvCurrent1Query(), cancellationToken),
            SolarEnergyToday = await _sender.Send(new GetSolarEnergyTodayQuery(), cancellationToken),
            SolarEnergyTotal = await _sender.Send(new GetSolarEnergyTotalQuery(), cancellationToken),
            InverterUseMode = await _sender.Send(new GetSolarChargerUseModeQuery(), cancellationToken),
            PowerControlMode = await _sender.Send(new GetModbusPowerControlQuery(), cancellationToken)
        };
    }
}
