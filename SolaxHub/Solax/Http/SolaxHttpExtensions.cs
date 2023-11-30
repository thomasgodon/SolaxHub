using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SolaxHub.Solax.Http
{
    internal static class SolaxHttpExtensions
    {
        public static SolaxData ToSolaxData(this SolaxHttpResult result) =>
            new()
            {
                InverterSerialNumber = result.InverterSerialNumber,
                SerialNumber = result.SerialNumber,
                AcPower = result.AcPower,
                YieldToday = result.YieldToday,
                YieldTotal = result.YieldTotal,
                FeedInPower = result.FeedInPower,
                FeedInEnergy = result.FeedInEnergy,
                ConsumeEnergy = result.ConsumeEnergy,
                FeedInPowerM2 = result.FeedInPowerM2,
                Soc = result.Soc,
                EpsPowerR = result.EpsPowerR,
                EpsPowerS = result.EpsPowerS,
                EpsPowerT = result.EpsPowerT,
                InverterType = result.InverterType,
                InverterStatus = result.InverterStatus,
                BatteryPower = result.BatteryPower,
                PvPowerMppt1 = result.PvPowerMppt1,
                PvPowerMppt2 = result.PvPowerMppt2,
                PvPowerMppt3 = result.PvPowerMppt3,
                PvPowerMppt4 = result.PvPowerMppt4,
                InverterUseMode = result.InverterUseMode
            };
    }
}
