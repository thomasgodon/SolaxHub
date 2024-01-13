using SolaxHub.IotHub.Models;
using SolaxHub.Solax.Modbus;
using SolaxHub.Solax.Models;

namespace SolaxHub.IotHub.Extensions
{
    internal static class IotHubExtensions
    {
        public static DeviceData ToDeviceData(this SolaxData data) =>
            new()
            {
                InverterSerialNumber = data.InverterSerialNumber,
                SerialNumber = data.RegistrationCodePocket,
                Soc = data.BatteryCapacity,
                BatteryPower = data.BatteryPower,
                AcPower = data.InverterPower,
                FeedInPower = data.FeedInPower,
                InverterStatus = data.InverterStatus,
                InverterType = data.InverterSerialNumber.ToSolaxInverterType(),
                EpsPowerR = data.PvPower1,
                YieldToday = data.SolarEnergyToday,
                YieldTotal = data.SolarEnergyTotal,
                HouseLoad = data.HouseLoad,
                InverterUseMode = data.SolaxInverterUseMode,
                ConsumeEnergy = data.ConsumeEnergy,
                FeedInEnergy = data.FeedInEnergy,
                BatteryOutputEnergyToday = data.BatteryOutputEnergyToday,
                BatteryInputEnergyToday = data.BatteryInputEnergyToday
            };
    }
}
