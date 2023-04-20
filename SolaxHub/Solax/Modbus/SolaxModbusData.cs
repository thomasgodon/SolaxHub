using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaxHub.Solax.Modbus
{
    internal class SolaxModbusData
    {
        public string InverterSerialNumber { get; init; } = default!;
        public string RegistrationCodePocket { get; init; } = default!;
        public ushort BatteryCapacity { get; init; } = default!;
        public short BatteryPower { get; init; } = default!;
        public ushort InverterVoltage { get; init; } = default!;
        public short InverterPower { get; init; } = default!;
        public ushort GridImport => (ushort)(FeedInPower < 0 ? (ushort)Math.Abs(FeedInPower) : 0);
        public ushort GridExport => (ushort)(FeedInPower >= 0 ? (ushort)Math.Abs(FeedInPower) : 0);
        public int FeedInPower { get; init; } = default!;
        public double ConsumeEnergy { get; init; } = default!;
        public double FeedInEnergy { get; init; } = default!;
        public ushort RunMode { get; init;} = default!;
        public ushort PvPower1 { get; init; } = default!;
        public ushort PvVolt1 { get; init; } = default!;
        public ushort PvCurrent1 { get; init; } = default!;
        public double SolarEnergyToday { get; init; } = default!;
        public double SolarEnergyTotal { get; init; } = default!;
        public ushort SolarChargerUseMode { get; init; } = default!;
    }
}
