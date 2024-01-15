using SolaxHub.Solax.Extensions;

namespace SolaxHub.Solax.Models
{
    internal class SolaxData
    {
        public string InverterSerialNumber { get; init; } = default!;
        public SolaxInverterType InverterType => InverterSerialNumber.ToSolaxInverterType();
        public string RegistrationCodePocket { get; init; } = default!;
        public ushort BatteryCapacity { get; init; } = default!;
        public short BatteryPower { get; init; } = default!;
        public ushort InverterVoltage { get; init; } = default!;
        public short InverterPower { get; init; } = default!;
        public int FeedInPower { get; init; } = default!;
        public double ConsumeEnergy { get; init; } = default!;
        public double FeedInEnergy { get; init; } = default!;
        public SolaxInverterStatus InverterStatus { get; init; } = default!;
        public ushort PvPower1 { get; init; } = default!;
        public ushort PvVolt1 { get; init; } = default!;
        public ushort PvCurrent1 { get; init; } = default!;
        public double SolarEnergyToday { get; init; } = default!;
        public double SolarEnergyTotal { get; init; } = default!;
        public SolaxInverterUseMode SolaxInverterUseMode { get; init; } = default!;
        public double BatteryOutputEnergyToday { get; init; } = default!;
        public double BatteryInputEnergyToday { get; init; } = default!;
        public double HouseLoad => InverterPower - FeedInPower;
    }
}
