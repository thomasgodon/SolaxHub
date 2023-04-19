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
        public ushort BatteryCapacity { get; init; } = default!;
        public short BatteryPower { get; init; } = default!;
        public ushort GridVoltage { get; init; } = default!;
        public short InverterPower { get; init; } = default!;
        public ushort GridImport => (ushort)(FeedInPower < 0 ? (ushort)Math.Abs(FeedInPower) : 0);
        public ushort GridExport => (ushort)(FeedInPower >= 0 ? (ushort)Math.Abs(FeedInPower) : 0);
        public int FeedInPower { get; init; } = default!;
        public ushort RunMode { get; init;} = default!;
        public ushort EpsPowerActiveR { get; init; } = default!;
        public ushort EpsVoltR { get; init; } = default!;
        public ushort EpsCurrentR { get; init; } = default!;
        public ushort SolarEnergyToday { get; init; } = default!;
        public int SolarEnergyTotal { get; init; } = default!;
        public ushort SolarChargerUseMode { get; init; } = default!;
    }
}
