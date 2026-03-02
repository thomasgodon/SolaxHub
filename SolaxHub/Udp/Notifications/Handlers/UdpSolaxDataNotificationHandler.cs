using MediatR;
using SolaxHub.Solax.Models;
using SolaxHub.Solax.Notifications;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Options;
using SolaxHub.Udp.Models;

namespace SolaxHub.Udp.Notifications.Handlers;

internal class UdpSolaxDataNotificationHandler : INotificationHandler<SolaxDataArrivedNotification>
{
    private readonly UdpOptions _options;

    public UdpSolaxDataNotificationHandler(IOptions<UdpOptions> options)
    {
        _options = options.Value;
    }

    public async Task Handle(SolaxDataArrivedNotification notification, CancellationToken cancellationToken)
    {
        if (_options.Enabled is false)
        {
            return;
        }

        foreach (var (udpData, port) in GenerateUdpMessages(notification.Data, _options.PortMapping).ToList())
        {
            using var udpSender = new UdpClient();
            udpSender.Connect(_options.Host, port);
            await udpSender.SendAsync(udpData, cancellationToken);
        }
    }

    private static IEnumerable<(byte[] Data, int Port)> GenerateUdpMessages(SolaxData data, IReadOnlyDictionary<string, int> portMapping)
    {
        foreach (var (name, value) in GetMappableValues(data))
        {
            if (portMapping.TryGetValue(name, out var port))
            {
                yield return (Encoding.UTF8.GetBytes(value), port);
            }
        }
    }

    private static IEnumerable<(string Name, string Value)> GetMappableValues(SolaxData data)
    {
        yield return (nameof(SolaxData.BatteryCapacity), data.BatteryCapacity.ToString());
        yield return (nameof(SolaxData.BatteryPower), data.BatteryPower.ToString());
        yield return (nameof(SolaxData.InverterVoltage), data.InverterVoltage.ToString());
        yield return (nameof(SolaxData.InverterPower), data.InverterPower.ToString());
        yield return (nameof(SolaxData.FeedInPower), data.FeedInPower.ToString());
        yield return (nameof(SolaxData.ConsumeEnergy), data.ConsumeEnergy.ToString());
        yield return (nameof(SolaxData.FeedInEnergy), data.FeedInEnergy.ToString());
        yield return (nameof(SolaxData.InverterStatus), ((int)data.InverterStatus).ToString());
        yield return (nameof(SolaxData.PvPower1), data.PvPower1.ToString());
        yield return (nameof(SolaxData.PvVolt1), data.PvVolt1.ToString());
        yield return (nameof(SolaxData.PvCurrent1), data.PvCurrent1.ToString());
        yield return (nameof(SolaxData.SolarEnergyToday), data.SolarEnergyToday.ToString());
        yield return (nameof(SolaxData.SolarEnergyTotal), data.SolarEnergyTotal.ToString());
        yield return (nameof(SolaxData.InverterUseMode), ((int)data.InverterUseMode).ToString());
        yield return (nameof(SolaxData.BatteryOutputEnergyToday), data.BatteryOutputEnergyToday.ToString());
        yield return (nameof(SolaxData.BatteryInputEnergyToday), data.BatteryInputEnergyToday.ToString());
        yield return (nameof(SolaxData.BatteryOutputEnergyTotal), data.BatteryOutputEnergyTotal.ToString());
        yield return (nameof(SolaxData.BatteryInputEnergyTotal), data.BatteryInputEnergyTotal.ToString());
        yield return (nameof(SolaxData.HouseLoad), data.HouseLoad.ToString());
        yield return (nameof(SolaxData.PowerControlMode), ((int)data.PowerControlMode).ToString());
        yield return (nameof(SolaxData.LockState), ((int)data.LockState).ToString());
    }
}
