namespace SolaxHub.Application.Dashboard;

/// <summary>
/// Flat, web-friendly projection of the inverter aggregate plus integration connection states,
/// pushed to the dashboard page on every refresh. Units are normalised here (e.g. solar V/A are
/// scaled from the raw 0.1-unit registers) so the front-end can render values verbatim.
/// </summary>
public record DashboardSnapshot
{
    public required DateTimeOffset Timestamp { get; init; }

    public required string SerialNumber { get; init; }
    public required string Type { get; init; }
    public required string Status { get; init; }
    public required string UseMode { get; init; }
    public required string LockState { get; init; }
    public required string PowerControlMode { get; init; }

    /// <summary>Inverter AC output power, watts.</summary>
    public required int InverterPower { get; init; }
    /// <summary>Inverter AC voltage, volts.</summary>
    public required int InverterVoltage { get; init; }
    /// <summary>Calculated household consumption, watts.</summary>
    public required int HouseLoad { get; init; }

    public required SolarDto Solar { get; init; }
    public required BatteryDto Battery { get; init; }
    public required GridDto Grid { get; init; }
    public required ConnectionsDto Connections { get; init; }
}

/// <summary>Solar (PV string 1) values. Voltage/Current scaled to V/A; energy in kWh.</summary>
public record SolarDto
{
    public required double Voltage { get; init; }
    public required double Current { get; init; }
    public required int Power { get; init; }
    public required double EnergyToday { get; init; }
    public required double EnergyTotal { get; init; }
}

/// <summary>Battery values. Power watts (negative = discharge, positive = charge); energy in kWh.</summary>
public record BatteryDto
{
    public required int Power { get; init; }
    public required int Capacity { get; init; }
    public required double OutputToday { get; init; }
    public required double InputToday { get; init; }
    public required double OutputTotal { get; init; }
    public required double InputTotal { get; init; }
}

/// <summary>Grid values. FeedInPower watts (positive = export, negative = import); energy in kWh.</summary>
public record GridDto
{
    public required int FeedInPower { get; init; }
    public required double FeedInEnergy { get; init; }
    public required double ConsumeEnergy { get; init; }
}

/// <summary>Per-integration connectivity, synthesised for the dashboard (no central health service exists).</summary>
public record ConnectionsDto
{
    public required ModbusConnectionDto Modbus { get; init; }
    public required KnxConnectionDto Knx { get; init; }
    public required UdpConnectionDto Udp { get; init; }
}

public record ModbusConnectionDto
{
    public required bool Connected { get; init; }
}

public record KnxConnectionDto
{
    public required bool Enabled { get; init; }
    public required bool Connected { get; init; }
}

public record UdpConnectionDto
{
    public required bool Enabled { get; init; }
}
