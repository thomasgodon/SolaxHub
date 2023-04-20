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
                HouseLoad = data.GridImport + data.InverterPower,
                BatteryStatus = data.SolarChargerUseMode.ToSolaxBatteryStatus(),
                ConsumeEnergy = data.ConsumeEnergy,
                FeedInEnergy = data.FeedInEnergy
            };

        private static SolaxInverterType ToSolaxInverterType(this string serialNumber) =>
            serialNumber switch
            {
                { } when serialNumber.StartsWith("H43")  => SolaxInverterType.X1HybridG4, // HYBRID | GEN4 | X1 # Gen4 X1 3kW / 3.7kW
                _ => SolaxInverterType.Unknown
            };

        private static SolaxBatteryStatus ToSolaxBatteryStatus(this ushort chargerUseMode)
        {
            if (chargerUseMode == 0) return SolaxBatteryStatus.SelfUseMode;
            if (chargerUseMode == 1) return SolaxBatteryStatus.FeedInPriority;
            if (chargerUseMode == 2) return SolaxBatteryStatus.BackUpMode;
            if (chargerUseMode == 3) return SolaxBatteryStatus.ForceTimeUse;
            return SolaxBatteryStatus.Unknown;
        }
    }
}
