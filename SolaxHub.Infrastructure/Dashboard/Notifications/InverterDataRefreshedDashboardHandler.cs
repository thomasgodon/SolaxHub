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
            InverterTemperature = inverter.InverterTemperature,
            RadiatorTemperature = inverter.RadiatorTemperature,
            Solar = new SolarDto
            {
                // Raw registers are in 0.1 V / 0.1 A units.
                Voltage = inverter.Solar.Voltage1 / 10.0,
                Current = inverter.Solar.Current1 / 10.0,
                Power = inverter.Solar.Power1,
                EnergyToday = inverter.Solar.EnergyToday,
                EnergyTotal = inverter.Solar.EnergyTotal,
                Voltage2 = inverter.Solar.Voltage2 / 10.0,
                Current2 = inverter.Solar.Current2 / 10.0,
                Power2 = inverter.Solar.Power2,
                PowerTotal = inverter.Solar.Power1 + inverter.Solar.Power2
            },
            Battery = new BatteryDto
            {
                Power = inverter.Battery.Power,
                Capacity = inverter.Battery.Capacity,
                OutputToday = inverter.Battery.OutputToday,
                InputToday = inverter.Battery.InputToday,
                OutputTotal = inverter.Battery.OutputTotal,
                InputTotal = inverter.Battery.InputTotal,
                Voltage = inverter.Battery.Voltage / 100.0,
                Current = inverter.Battery.Current / 10.0,
                Temperature = inverter.Battery.Temperature / 10.0
            },
            Grid = new GridDto
            {
                FeedInPower = inverter.Grid.FeedInPower,
                FeedInEnergy = inverter.Grid.FeedInEnergy,
                ConsumeEnergy = inverter.Grid.ConsumeEnergy,
                Frequency = inverter.Grid.Frequency / 100.0,
                Current = inverter.Grid.Current / 10.0,
                Phases = MapPhases(inverter.Grid)
            },
            Eps = MapEps(inverter.Eps),
            Faults = new FaultsDto
            {
                InverterFault = inverter.Faults.InverterFault,
                ChargerFault = inverter.Faults.ChargerFault,
                ManagerFault = inverter.Faults.ManagerFault,
                BmsWarning = inverter.Faults.BmsWarning,
                HasFault = inverter.Faults.HasFault
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

    /// <summary>Projects per-phase grid data (three-phase only); returns null when no phase data is present.</summary>
    private static GridPhaseDto[]? MapPhases(Domain.Inverter.GridState grid)
    {
        if (grid.PhaseR is null && grid.PhaseS is null && grid.PhaseT is null)
            return null;

        var phases = new List<GridPhaseDto>(3);
        if (grid.PhaseR is { } r) phases.Add(new GridPhaseDto { Name = "L1", Current = r.Current / 10.0, Power = r.Power });
        if (grid.PhaseS is { } s) phases.Add(new GridPhaseDto { Name = "L2", Current = s.Current / 10.0, Power = s.Power });
        if (grid.PhaseT is { } t) phases.Add(new GridPhaseDto { Name = "L3", Current = t.Current / 10.0, Power = t.Power });
        return phases.ToArray();
    }

    /// <summary>Projects EPS output; returns null when the inverter reports no backup output.</summary>
    private static EpsDto? MapEps(Domain.Inverter.EpsState eps)
    {
        if (eps.Voltage == 0 && eps.Power == 0 && eps.Current == 0)
            return null;

        return new EpsDto
        {
            Voltage = eps.Voltage / 10.0,
            Current = eps.Current / 10.0,
            Power = eps.Power,
            Frequency = eps.Frequency / 100.0
        };
    }
}
