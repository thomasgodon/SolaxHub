namespace SolaxHub.Solax.Modbus
{
    internal static class SolaxModbusExtensions
    {
        public static SolaxData ToSolaxData(this SolaxModbusData data) =>
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
                TodayBatteryOutputEnergy = data.BatteryOutputEnergyToday
            };

        private static SolaxInverterType ToSolaxInverterType(this string serialNumber) =>
            serialNumber switch
            {
                not null when serialNumber.StartsWith("H43")  => SolaxInverterType.X1HybridG4, // HYBRID | GEN4 | X1 # Gen4 X1 3kW / 3.7kW
                _ => SolaxInverterType.Unknown
            };

        private static SolaxInverterUseMode ToSolaxBatteryStatus(this ushort chargerUseMode)
            => chargerUseMode switch
            {
                0 => SolaxInverterUseMode.SelfUseMode,
                1 => SolaxInverterUseMode.FeedInPriority,
                2 => SolaxInverterUseMode.BackUpMode,
                3 => SolaxInverterUseMode.ForceTimeUse,
                _ => SolaxInverterUseMode.Unknown
            };
    }
}
