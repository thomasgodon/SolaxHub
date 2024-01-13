﻿using SolaxHub.Solax.Models;

namespace SolaxHub.Solax.Modbus
{
    internal static class SolaxModbusExtensions
    {
        public static SolaxInverterType ToSolaxInverterType(this string serialNumber) =>
            serialNumber switch
            {
                not null when serialNumber.StartsWith("H43")  => SolaxInverterType.X1HybridG4, // HYBRID | GEN4 | X1 # Gen4 X1 3kW / 3.7kW
                _ => SolaxInverterType.Unknown
            };

        public static SolaxInverterUseMode ToSolaxInverterUseMode(this ushort chargerUseMode)
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
