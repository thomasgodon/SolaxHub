using MediatR;
using Microsoft.Extensions.Logging;

namespace SolaxHub.Application.Inverter.Notifications.Handlers;

internal class InverterDataRefreshedDebugLogHandler(ILogger<InverterDataRefreshedDebugLogHandler> logger)
    : INotificationHandler<InverterDataRefreshed>
{
    public Task Handle(InverterDataRefreshed notification, CancellationToken cancellationToken)
    {
        var inv = notification.Inverter;

        logger.LogDebug(
            "[Inverter] Status={Status} UseMode={UseMode} LockState={LockState} PowerControlMode={PowerControlMode} | " +
            "Power={InverterPower}W Voltage={InverterVoltage}V HouseLoad={HouseLoad}W | " +
            "Battery Power={BatteryPower}W Capacity={BatteryCapacity}% OutToday={BatteryOutToday}kWh InToday={BatteryInToday}kWh OutTotal={BatteryOutTotal}kWh InTotal={BatteryInTotal}kWh | " +
            "Solar V1={SolarVoltage1}V I1={SolarCurrent1}A P1={SolarPower1}W Today={SolarEnergyToday}kWh Total={SolarEnergyTotal}kWh | " +
            "Grid FeedIn={FeedInPower}W FeedInEnergy={FeedInEnergy}kWh ConsumeEnergy={ConsumeEnergy}kWh",
            inv.Status, inv.UseMode, inv.LockState, inv.PowerControlMode,
            inv.InverterPower, inv.InverterVoltage, inv.HouseLoad,
            inv.Battery.Power, inv.Battery.Capacity, inv.Battery.OutputToday, inv.Battery.InputToday, inv.Battery.OutputTotal, inv.Battery.InputTotal,
            inv.Solar.Voltage1, inv.Solar.Current1, inv.Solar.Power1, inv.Solar.EnergyToday, inv.Solar.EnergyTotal,
            inv.Grid.FeedInPower, inv.Grid.FeedInEnergy, inv.Grid.ConsumeEnergy);

        return Task.CompletedTask;
    }
}
