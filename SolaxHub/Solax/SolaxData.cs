using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SolaxHub.Solax
{
    internal class SolaxData
    {
        [JsonProperty("inverterSN")]
        public string InverterSerialNumber { get; init; } = default!;
        [JsonProperty("sn")]
        public string SerialNumber { get; init; } = default!;
        [JsonProperty("acpower")]
        public double AcPower { get; init; }
        [JsonProperty("houseload")]
        public double HouseLoad { get; init; }
        [JsonProperty("yieldtoday")]
        public double YieldToday { get; init; }
        [JsonProperty("yieldtotal")]
        public double YieldTotal { get; init; }
        [JsonProperty("feedinpower")]
        public double FeedInPower { get; init; }
        [JsonProperty("feedinenergy")]
        public double FeedInEnergy { get; init; }
        [JsonProperty("consumeenergy")]
        public double ConsumeEnergy { get; init; }
        [JsonProperty("feedinpowerM2")]
        public double FeedInPowerM2 { get; init; }
        [JsonProperty("soc")]
        public double Soc { get; init; }
        [JsonProperty("peps1")]
        public double? EpsPowerR { get; init; }
        [JsonProperty("peps2")]
        public double? EpsPowerS { get; init; }
        [JsonProperty("peps3")]
        public double? EpsPowerT { get; init; }
        [JsonProperty("inverterType")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SolaxInverterType InverterType { get; init; }
        [JsonProperty("inverterStatus")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SolaxInverterStatus InverterStatus { get; init; }
        [JsonProperty("batPower")]
        public double BatteryPower { get; init; }
        [JsonProperty("powerdc1")]
        public double? PvPowerMppt1 { get; init; }
        [JsonProperty("powerdc2")]
        public double? PvPowerMppt2 { get; init; }
        [JsonProperty("powerdc3")]
        public double? PvPowerMppt3 { get; init; }
        [JsonProperty("powerdc4")]
        public double? PvPowerMppt4 { get; init; }
        [JsonProperty("batStatus")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SolaxInverterUseMode BatteryStatus => InverterUseMode;
        [JsonProperty("inverterUseMode")] 
        [JsonConverter(typeof(StringEnumConverter))]
        public SolaxInverterUseMode InverterUseMode { get; init; }
        [JsonProperty("sourceTimestamp")]
        public DateTimeOffset SourceTimestamp { get; } = DateTimeOffset.UtcNow;
        [JsonProperty("batteryOutputEnergyToday")]
        public double BatteryOutputEnergyToday { get; init; }
        [JsonProperty("BatteryInputEnergyToday")]
        public double BatteryInputEnergyToday { get; init; }
    }
}
