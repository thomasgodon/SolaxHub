using SolaxHub.Domain.Common;
using SolaxHub.Domain.Inverter.Events;

namespace SolaxHub.Domain.Inverter;

public sealed class Inverter : AggregateRoot
{
    private Inverter() { }

    public static Inverter Create() => new();

    public string SerialNumber { get; private set; } = string.Empty;
    public InverterType Type { get; private set; } = InverterType.Unknown;
    public InverterStatus Status { get; private set; } = InverterStatus.WaitMode;
    public InverterUseMode UseMode { get; private set; } = InverterUseMode.Unknown;
    public LockState LockState { get; private set; } = LockState.Locked;
    public PowerControlMode PowerControlMode { get; private set; } = PowerControlMode.Disabled;
    public BatteryState Battery { get; private set; } = new(0, 0, 0, 0, 0, 0);
    public SolarState Solar { get; private set; } = new(0, 0, 0, 0, 0);
    public GridState Grid { get; private set; } = new(0, 0, 0);
    public int InverterPower { get; private set; }
    public ushort InverterVoltage { get; private set; }
    public string RegistrationCode { get; private set; } = string.Empty;
    public short InverterTemperature { get; private set; }
    public short RadiatorTemperature { get; private set; }
    public EpsState Eps { get; private set; } = new(0, 0, 0, 0);
    public FaultState Faults { get; private set; } = new(0, 0, 0, 0);

    public int HouseLoad => InverterPower - Grid.FeedInPower;

    /// <summary>True for three-phase (X3) inverters; gates per-phase grid data.</summary>
    public bool IsThreePhase => Type is InverterType.X3HybridG4 or InverterType.X3HybiydFit;

    public void Refresh(InverterSnapshot snapshot)
    {
        SerialNumber = snapshot.SerialNumber;
        Type = ResolveType(snapshot.SerialNumber);
        Status = snapshot.Status;
        UseMode = snapshot.UseMode;
        LockState = snapshot.LockState;
        PowerControlMode = snapshot.PowerControlMode;
        Battery = snapshot.Battery;
        Solar = snapshot.Solar;
        Grid = snapshot.Grid;
        InverterPower = snapshot.InverterPower;
        InverterVoltage = snapshot.InverterVoltage;
        RegistrationCode = snapshot.RegistrationCode;
        InverterTemperature = snapshot.InverterTemperature;
        RadiatorTemperature = snapshot.RadiatorTemperature;
        Eps = snapshot.Eps;
        Faults = snapshot.Faults;

        AddDomainEvent(new InverterDataRefreshed(this));
    }

    private static InverterType ResolveType(string serialNumber) =>
        serialNumber switch
        {
            not null when serialNumber.StartsWith("H34") => InverterType.X3HybridG4,
            not null when serialNumber.StartsWith("H43")
                       || serialNumber.StartsWith("H450")
                       || serialNumber.StartsWith("H460")
                       || serialNumber.StartsWith("H475") => InverterType.X1HybridG4,
            _ => InverterType.Unknown
        };
}
