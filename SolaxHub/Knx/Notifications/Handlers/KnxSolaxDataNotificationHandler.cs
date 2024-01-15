using SolaxHub.Knx.Client;
using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Knx.Models;
using SolaxHub.Knx.Services;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Notifications;

namespace SolaxHub.Knx.Notifications.Handlers
{
    internal class KnxSolaxDataNotificationHandler : INotificationHandler<SolaxDataArrivedNotification>
    {
        private readonly IKnxClient _knxClient;
        private readonly KnxOptions _knxOptions;
        private readonly IKnxValueBufferService _knxValueBufferService;

        public KnxSolaxDataNotificationHandler(
            IKnxClient knxClient,
            IOptions<KnxOptions> options,
            IKnxValueBufferService knxValueBufferService)
        {
            _knxClient = knxClient;
            _knxOptions = options.Value;
            _knxValueBufferService = knxValueBufferService;
        }

        public async Task Handle(SolaxDataArrivedNotification notification, CancellationToken cancellationToken)
        {
            if (_knxOptions.Enabled is false)
            {
                return;
            }

            var values = UpdateValues(notification.Data).Where(m => m is not null);
            await _knxClient.SendValuesAsync(values!, cancellationToken);
        }

        private IEnumerable<KnxValue?> UpdateValues(SolaxData solaxData)
        {
            // HouseLoad - 14.056 power
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.HouseLoad), BitConverter.GetBytes((float)solaxData.HouseLoad));
            // AcPower - 14.056 power
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.InverterPower), BitConverter.GetBytes((float)solaxData.InverterPower));
            // BatteryPower - 14.056 power
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.BatteryPower), BitConverter.GetBytes((float)solaxData.BatteryPower));
            // SolarChargerUseMode - 6.020 status with mode
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.SolaxInverterUseMode), new[] { (byte)((int)solaxData.SolaxInverterUseMode * 2.55) });
            // ConsumeEnergy - 14.* 4byte float value
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.ConsumeEnergy), BitConverter.GetBytes((float)solaxData.ConsumeEnergy));
            // BatteryCapacity - 5.001 percentage
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.BatteryCapacity), new[] { (byte)(solaxData.BatteryCapacity * 2.55) });
            // EpsPower1 - 14.056 power
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.PvPower1), BitConverter.GetBytes((float)solaxData.PvPower1));
            // InverterStatus - 6.020 status with mode
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.InverterStatus), new[] { (byte)solaxData.InverterStatus });
            // SolarEnergyToday - 14
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.SolarEnergyToday), BitConverter.GetBytes((float)solaxData.SolarEnergyToday));
            // SolarEnergyTotal - 14
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.SolarEnergyTotal), BitConverter.GetBytes((float)solaxData.SolarEnergyTotal));
            // BatteryOutputEnergyToday - 14
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.BatteryOutputEnergyToday), BitConverter.GetBytes((float)solaxData.BatteryOutputEnergyToday));
            // BatteryInputEnergyToday - 14
            yield return _knxValueBufferService.UpdateValue(nameof(SolaxData.BatteryInputEnergyToday), BitConverter.GetBytes((float)solaxData.BatteryInputEnergyToday));
        }
    }
}
