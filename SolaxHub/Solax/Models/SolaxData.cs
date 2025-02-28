using SolaxHub.Solax.Extensions;

namespace SolaxHub.Solax.Models;

public record SolaxData
{
    public required string InverterSerialNumber { get; init; }
    public SolaxInverterType InverterType => InverterSerialNumber.ToSolaxInverterType();
    public required string RegistrationCodePocket { get; init; }
    public required ushort BatteryCapacity { get; init; }
    public required short BatteryPower { get; init; }
    public required ushort InverterVoltage { get; init; }
    public required short InverterPower { get; init; }
    public required int FeedInPower { get; init; }
    public required double ConsumeEnergy { get; init; }
    public required double FeedInEnergy { get; init; }
    public required SolaxInverterStatus InverterStatus { get; init; }
    public required ushort PvPower1 { get; init; }
    public required ushort PvVolt1 { get; init; }
    public required ushort PvCurrent1 { get; init; }
    public required double SolarEnergyToday { get; init; }
    public required double SolarEnergyTotal { get; init; }
    public required SolaxInverterUseMode InverterUseMode { get; init; }
    public required double BatteryOutputEnergyToday { get; init; }
    public required double BatteryInputEnergyToday { get; init; }
    public required double BatteryOutputEnergyTotal { get; init; }
    public required double BatteryInputEnergyTotal { get; init; }
    public double HouseLoad => InverterPower - FeedInPower;
    public required SolaxPowerControlMode PowerControlMode { get; init; }
    public required SolaxLockState LockState { get; init; }
}
