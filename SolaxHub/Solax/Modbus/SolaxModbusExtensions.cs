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
                BatteryStatus = data.SolarChargerUseMode.ToSolaxBatteryStatus(),
                ConsumeEnergy = data.ConsumeEnergy,
                FeedInEnergy = data.FeedInEnergy
            };

        private static SolaxInverterType ToSolaxInverterType(this string serialNumber) =>
            serialNumber switch
            {
                not null when serialNumber.StartsWith("H43")  => SolaxInverterType.X1HybridG4, // HYBRID | GEN4 | X1 # Gen4 X1 3kW / 3.7kW
                _ => SolaxInverterType.Unknown
            };

        private static SolaxBatteryStatus ToSolaxBatteryStatus(this ushort chargerUseMode)
            => chargerUseMode switch
            {
                0 => SolaxBatteryStatus.SelfUseMode,
                1 => SolaxBatteryStatus.FeedInPriority,
                2 => SolaxBatteryStatus.BackUpMode,
                3 => SolaxBatteryStatus.ForceTimeUse,
                _ => SolaxBatteryStatus.Unknown
            };
    }
}
