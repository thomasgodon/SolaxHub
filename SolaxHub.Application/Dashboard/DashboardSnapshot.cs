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
    /// <summary>Inverter internal temperature, °C.</summary>
    public required int InverterTemperature { get; init; }
    /// <summary>Radiator (heatsink) temperature, °C.</summary>
    public required int RadiatorTemperature { get; init; }

    public required SolarDto Solar { get; init; }
    public required BatteryDto Battery { get; init; }
    public required GridDto Grid { get; init; }
    /// <summary>EPS / backup output; null when the inverter reports no backup output.</summary>
    public EpsDto? Eps { get; init; }
    public required FaultsDto Faults { get; init; }
    public required ConnectionsDto Connections { get; init; }
}

/// <summary>Solar PV values. Voltage/Current scaled to V/A; energy in kWh. Strings 1 and 2.</summary>
public record SolarDto
{
    public required double Voltage { get; init; }
    public required double Current { get; init; }
    public required int Power { get; init; }
    public required double EnergyToday { get; init; }
    public required double EnergyTotal { get; init; }
    public required double Voltage2 { get; init; }
    public required double Current2 { get; init; }
    public required int Power2 { get; init; }
    /// <summary>Combined PV power across both strings, watts.</summary>
    public required int PowerTotal { get; init; }
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
    /// <summary>Battery voltage, volts.</summary>
    public required double Voltage { get; init; }
    /// <summary>Battery current, amps (positive = charging).</summary>
    public required double Current { get; init; }
    /// <summary>Battery temperature, °C.</summary>
    public required double Temperature { get; init; }
}

/// <summary>Grid values. FeedInPower watts (positive = export, negative = import); energy in kWh.</summary>
public record GridDto
{
    public required int FeedInPower { get; init; }
    public required double FeedInEnergy { get; init; }
    public required double ConsumeEnergy { get; init; }
    /// <summary>Grid frequency, Hz.</summary>
    public required double Frequency { get; init; }
    /// <summary>Grid current (phase R / single phase), amps.</summary>
    public required double Current { get; init; }
    /// <summary>Per-phase L1/L2/L3 values; null on single-phase inverters.</summary>
    public GridPhaseDto[]? Phases { get; init; }
}

/// <summary>Per-phase grid measurement (three-phase only).</summary>
public record GridPhaseDto
{
    public required string Name { get; init; }
    public required double Current { get; init; }
    public required int Power { get; init; }
}

/// <summary>EPS (off-grid / backup) output values. Null when the inverter reports no EPS output.</summary>
public record EpsDto
{
    public required double Voltage { get; init; }
    public required double Current { get; init; }
    public required int Power { get; init; }
    public required double Frequency { get; init; }
}

/// <summary>Raw inverter fault / warning codes plus a derived healthy flag.</summary>
public record FaultsDto
{
    public required long InverterFault { get; init; }
    public required int ChargerFault { get; init; }
    public required int ManagerFault { get; init; }
    public required int BmsWarning { get; init; }
    public required bool HasFault { get; init; }
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
