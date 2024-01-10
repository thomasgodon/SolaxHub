using SolaxHub.IotCentral.Models;
using SolaxHub.Solax.Modbus;
using SolaxHub.Solax.Models;

namespace SolaxHub.IotCentral.Extensions
{
    internal static class IotCentralExtensions
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
                InverterStatus = (SolaxInverterStatus)data.RunMode + 100,
                InverterType = data.InverterSerialNumber.ToSolaxInverterType(),
                EpsPowerR = data.PvPower1,
                YieldToday = data.SolarEnergyToday,
                YieldTotal = data.SolarEnergyTotal,
                HouseLoad = data.InverterPower - data.FeedInPower,
                InverterUseMode = data.SolarChargerUseMode.ToSolaxBatteryStatus(),
                ConsumeEnergy = data.ConsumeEnergy,
                FeedInEnergy = data.FeedInEnergy,
                BatteryOutputEnergyToday = data.BatteryOutputEnergyToday,
                BatteryInputEnergyToday = data.BatteryInputEnergyToday
            };
    }
}
