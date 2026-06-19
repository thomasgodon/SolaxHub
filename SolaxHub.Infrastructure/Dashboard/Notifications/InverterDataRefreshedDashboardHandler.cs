using MediatR;
using Microsoft.Extensions.Options;
using SolaxHub.Application.Dashboard;
using SolaxHub.Application.Dashboard.Options;
using SolaxHub.Application.Inverter.Notifications;
using SolaxHub.Infrastructure.Knx.Client;
using SolaxHub.Infrastructure.Knx.Options;
using SolaxHub.Infrastructure.Modbus.Client;
using SolaxHub.Infrastructure.Udp.Options;
using System.Text.Json;

namespace SolaxHub.Infrastructure.Dashboard.Notifications;

/// <summary>
/// Projects each inverter refresh (plus live integration connection states) into a
/// <see cref="DashboardSnapshot"/> and publishes it to the dashboard SSE broadcaster.
/// Lives in Infrastructure because connection state is read from the protocol clients.
/// </summary>
internal sealed class InverterDataRefreshedDashboardHandler : INotificationHandler<InverterDataRefreshed>
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IInverterSnapshotBroadcaster _broadcaster;
    private readonly ISolaxModbusClient _modbusClient;
    private readonly IKnxClient _knxClient;
    private readonly DashboardOptions _dashboardOptions;
    private readonly KnxOptions _knxOptions;
    private readonly UdpOptions _udpOptions;

    public InverterDataRefreshedDashboardHandler(
        IInverterSnapshotBroadcaster broadcaster,
        ISolaxModbusClient modbusClient,
        IKnxClient knxClient,
        IOptions<DashboardOptions> dashboardOptions,
        IOptions<KnxOptions> knxOptions,
        IOptions<UdpOptions> udpOptions)
    {
        _broadcaster = broadcaster;
        _modbusClient = modbusClient;
        _knxClient = knxClient;
        _dashboardOptions = dashboardOptions.Value;
        _knxOptions = knxOptions.Value;
        _udpOptions = udpOptions.Value;
    }

    public Task Handle(InverterDataRefreshed notification, CancellationToken cancellationToken)
    {
        if (_dashboardOptions.Enabled is false)
            return Task.CompletedTask;

        var inverter = notification.Inverter;

        var snapshot = new DashboardSnapshot
        {
            Timestamp = DateTimeOffset.UtcNow,
            SerialNumber = inverter.SerialNumber,
            Type = inverter.Type.ToString(),
            Status = inverter.Status.ToString(),
            UseMode = inverter.UseMode.ToString(),
            LockState = inverter.LockState.ToString(),
            PowerControlMode = inverter.PowerControlMode.ToString(),
            InverterPower = inverter.InverterPower,
            InverterVoltage = inverter.InverterVoltage,
            HouseLoad = inverter.HouseLoad,
            Solar = new SolarDto
            {
                // Raw registers are in 0.1 V / 0.1 A units.
                Voltage = inverter.Solar.Voltage1 / 10.0,
                Current = inverter.Solar.Current1 / 10.0,
                Power = inverter.Solar.Power1,
                EnergyToday = inverter.Solar.EnergyToday,
                EnergyTotal = inverter.Solar.EnergyTotal
            },
            Battery = new BatteryDto
            {
                Power = inverter.Battery.Power,
                Capacity = inverter.Battery.Capacity,
                OutputToday = inverter.Battery.OutputToday,
                InputToday = inverter.Battery.InputToday,
                OutputTotal = inverter.Battery.OutputTotal,
                InputTotal = inverter.Battery.InputTotal
            },
            Grid = new GridDto
            {
                FeedInPower = inverter.Grid.FeedInPower,
                FeedInEnergy = inverter.Grid.FeedInEnergy,
                ConsumeEnergy = inverter.Grid.ConsumeEnergy
            },
            Connections = new ConnectionsDto
            {
                Modbus = new ModbusConnectionDto { Connected = _modbusClient.IsConnected },
                Knx = new KnxConnectionDto { Enabled = _knxOptions.Enabled, Connected = _knxClient.IsConnected },
                Udp = new UdpConnectionDto { Enabled = _udpOptions.Enabled }
            }
        };

        _broadcaster.Publish(JsonSerializer.Serialize(snapshot, JsonOptions));

        return Task.CompletedTask;
    }
}
