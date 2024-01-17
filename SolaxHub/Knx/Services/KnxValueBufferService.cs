using Microsoft.Extensions.Options;
using SolaxHub.Knx.Extensions;
using SolaxHub.Knx.Models;
using SolaxHub.Solax.Models;

namespace SolaxHub.Knx.Services
{
    internal class KnxValueBufferService : IKnxValueBufferService
    {
        private readonly Dictionary<string, KnxValue> _capabilityKnxValueMapping;
        private readonly object _mappingLock = new();

        public KnxValueBufferService(IOptions<KnxOptions> options)
        {
            _capabilityKnxValueMapping = BuildCapabilityKnxValueMapping(options.Value);
        }


        public IEnumerable<KnxValue> UpdateKnxValues(SolaxData data)
        {
            lock (_mappingLock)
            {
                return UpdateValues(data).Where(m => m is not null).ToList()!;
            }
        }


        private KnxValue? UpdateValue(string capability, byte[] value)
        {
            if (_capabilityKnxValueMapping.TryGetValue(capability, out var knxSolaxValue) is false)
            {
                return null;
            }

            if (knxSolaxValue.Value is not null)
            {
                if (knxSolaxValue.Value.SequenceEqual(value))
                {
                    return null;
                }
            }

            _capabilityKnxValueMapping[capability].Value = value;
            return _capabilityKnxValueMapping[capability];
        }

        public IReadOnlyDictionary<string, KnxValue> GetKnxValues()
        {
            lock (_mappingLock)
            {
                return _capabilityKnxValueMapping.ToDictionary(key => key.Key, value => value.Value);
            }
        }

        private static Dictionary<string, KnxValue> BuildCapabilityKnxValueMapping(KnxOptions options)
        {
            var solaxData = new Dictionary<string, KnxValue>(options.ReadGroupAddresses.Count);

            foreach (var groupAddressMapping in options.GetReadGroupAddressesFromOptions())
            {
                solaxData.Add(groupAddressMapping.Key, new KnxValue(groupAddressMapping.Value));
            }

            return solaxData;
        }

        private IEnumerable<KnxValue?> UpdateValues(SolaxData solaxData)
        {
            // HouseLoad - 14.056 power
            yield return UpdateValue(nameof(SolaxData.HouseLoad), BitConverter.GetBytes((float)solaxData.HouseLoad));
            // AcPower - 14.056 power
            yield return UpdateValue(nameof(SolaxData.InverterPower), BitConverter.GetBytes((float)solaxData.InverterPower));
            // BatteryPower - 14.056 power
            yield return UpdateValue(nameof(SolaxData.BatteryPower), BitConverter.GetBytes((float)solaxData.BatteryPower));
            // SolarChargerUseMode - 6.020 status with mode
            yield return UpdateValue(nameof(SolaxData.SolaxInverterUseMode), new[] { (byte)((int)solaxData.SolaxInverterUseMode * 2.55) });
            // ConsumeEnergy - 14.* 4byte float value
            yield return UpdateValue(nameof(SolaxData.ConsumeEnergy), BitConverter.GetBytes((float)solaxData.ConsumeEnergy));
            // BatteryCapacity - 5.001 percentage
            yield return UpdateValue(nameof(SolaxData.BatteryCapacity), new[] { (byte)(solaxData.BatteryCapacity * 2.55) });
            // EpsPower1 - 14.056 power
            yield return UpdateValue(nameof(SolaxData.PvPower1), BitConverter.GetBytes((float)solaxData.PvPower1));
            // InverterStatus - 6.020 status with mode
            yield return UpdateValue(nameof(SolaxData.InverterStatus), new[] { (byte)solaxData.InverterStatus });
            // SolarEnergyToday - 14
            yield return UpdateValue(nameof(SolaxData.SolarEnergyToday), BitConverter.GetBytes((float)solaxData.SolarEnergyToday));
            // SolarEnergyTotal - 14
            yield return UpdateValue(nameof(SolaxData.SolarEnergyTotal), BitConverter.GetBytes((float)solaxData.SolarEnergyTotal));
            // BatteryOutputEnergyToday - 14
            yield return UpdateValue(nameof(SolaxData.BatteryOutputEnergyToday), BitConverter.GetBytes((float)solaxData.BatteryOutputEnergyToday));
            // BatteryInputEnergyToday - 14
            yield return UpdateValue(nameof(SolaxData.BatteryInputEnergyToday), BitConverter.GetBytes((float)solaxData.BatteryInputEnergyToday));
            // PowerControl - 1.001 switch
            yield return UpdateValue(nameof(SolaxData.PowerControl), BitConverter.GetBytes(solaxData.PowerControl));
        }
    }
}
