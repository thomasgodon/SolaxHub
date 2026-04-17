using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Application.Inverter.Notifications;
using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Udp.Options;
using System.Net.Sockets;
using System.Text;

namespace SolaxHub.Infrastructure.Udp.Notifications;

internal class InverterDataRefreshedUdpHandler : INotificationHandler<InverterDataRefreshed>
{
    private readonly UdpOptions _options;

    public InverterDataRefreshedUdpHandler(IOptions<UdpOptions> options)
    {
        _options = options.Value;
    }

    public async Task Handle(InverterDataRefreshed notification, CancellationToken cancellationToken)
    {
        if (_options.Enabled is false)
            return;

        foreach (var (data, port) in GenerateUdpMessages(notification.Inverter, _options.PortMapping).ToList())
        {
            using var udpSender = new UdpClient();
            udpSender.Connect(_options.Host, port);
            await udpSender.SendAsync(data, cancellationToken);
        }
    }

    private static IEnumerable<(byte[] Data, int Port)> GenerateUdpMessages(
        Inverter inverter, IReadOnlyDictionary<string, int> portMapping)
    {
        foreach (var (name, value) in GetMappableValues(inverter))
        {
            if (portMapping.TryGetValue(name, out var port))
                yield return (Encoding.UTF8.GetBytes(value), port);
        }
    }

    private static IEnumerable<(string Name, string Value)> GetMappableValues(Inverter inverter)
    {
        yield return ("BatteryCapacity", inverter.Battery.Capacity.ToString());
        yield return ("BatteryPower", inverter.Battery.Power.ToString());
        yield return ("InverterVoltage", inverter.InverterVoltage.ToString());
        yield return ("InverterPower", inverter.InverterPower.ToString());
        yield return ("FeedInPower", inverter.Grid.FeedInPower.ToString());
        yield return ("ConsumeEnergy", inverter.Grid.ConsumeEnergy.ToString());
        yield return ("FeedInEnergy", inverter.Grid.FeedInEnergy.ToString());
        yield return ("InverterStatus", ((int)inverter.Status).ToString());
        yield return ("PvPower1", inverter.Solar.Power1.ToString());
        yield return ("PvVolt1", inverter.Solar.Voltage1.ToString());
        yield return ("PvCurrent1", inverter.Solar.Current1.ToString());
        yield return ("SolarEnergyToday", inverter.Solar.EnergyToday.ToString());
        yield return ("SolarEnergyTotal", inverter.Solar.EnergyTotal.ToString());
        yield return ("InverterUseMode", ((int)inverter.UseMode).ToString());
        yield return ("BatteryOutputEnergyToday", inverter.Battery.OutputToday.ToString());
        yield return ("BatteryInputEnergyToday", inverter.Battery.InputToday.ToString());
        yield return ("BatteryOutputEnergyTotal", inverter.Battery.OutputTotal.ToString());
        yield return ("BatteryInputEnergyTotal", inverter.Battery.InputTotal.ToString());
        yield return ("HouseLoad", inverter.HouseLoad.ToString());
        yield return ("PowerControlMode", ((int)inverter.PowerControlMode).ToString());
        yield return ("LockState", ((int)inverter.LockState).ToString());
    }
}
