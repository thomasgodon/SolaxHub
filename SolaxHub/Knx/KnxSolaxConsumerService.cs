using Microsoft.Extensions.Options;
using SolaxHub.Solax;
using SolaxHub.Knx.Client;
using Knx.Falcon;
using SolaxHub.Knx.Models;
using SolaxHub.Solax.Models;

namespace SolaxHub.Knx
{
    internal class KnxWriterService : ISolaxConsumer
    {
        private readonly KnxOptions _options; 
        private readonly IKnxClient _knxClient;
        private readonly Dictionary<string, KnxValue> _knxValueBuffer;
        private readonly Dictionary<GroupAddress, string> _capabilityAddressMapping;
        private readonly object _solaxDataLock = new();

        public bool Enabled => _options.Enabled;

        public KnxWriterService(IOptions<KnxOptions> options, IKnxClient knxClient)
        {
            _options = options.Value;
            _knxValueBuffer = BuildKnxSolaxValueBuffer(_options);
            _capabilityAddressMapping = BuildCapabilityReadAddressMapping(_options);
            _knxClient = knxClient;
        }

        public async Task ConsumeSolaxDataAsync(SolaxData data, CancellationToken cancellationToken)
        {
            if (Enabled is false)
            {
                return;
            }

            var updatedValues = UpdateValues(data)
                .Where(m => m is not null).ToList() as IEnumerable<KnxValue>;

            await _knxClient.SendValuesAsync(updatedValues, cancellationToken);
        }

        private KnxValue? UpdateValue(string capability, byte[] value, bool isShort = false)
        {
            if (_knxValueBuffer.TryGetValue(capability, out var knxSolaxValue) is false)
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

            _knxValueBuffer[capability].Value = value;
            _knxValueBuffer[capability].IsShort = isShort;
            return _knxValueBuffer[capability];
        }

        private IEnumerable<KnxValue?> UpdateValues(SolaxData solaxData)
        {
            lock (_solaxDataLock)
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
            }
        }

        private static Dictionary<string, KnxValue> BuildKnxSolaxValueBuffer(KnxOptions knxOptions)
        {
            var solaxData = new Dictionary<string, KnxValue>(knxOptions.ReadGroupAddresses.Count);

            foreach (var groupAddressMapping in GetReadGroupAddressesFromOptions(knxOptions))
            {
                solaxData.Add(groupAddressMapping.Key, new KnxValue(groupAddressMapping.Value));
            }

            return solaxData;
        }

        private static IEnumerable<KeyValuePair<string, string>> GetReadGroupAddressesFromOptions(KnxOptions options)
            => options.ReadGroupAddresses
                .Where(
                    mapping => string.IsNullOrEmpty(mapping.Value) is false);

        private static Dictionary<GroupAddress, string> BuildCapabilityReadAddressMapping(KnxOptions knxOptions)
            => GetReadGroupAddressesFromOptions(knxOptions)
                .ToDictionary(
                    groupAddressMapping => GroupAddress.Parse(groupAddressMapping.Value),
                    groupAddressMapping => groupAddressMapping.Key);

        public KnxValue? ReadValue(GroupAddress address)
        {
            if (_capabilityAddressMapping.TryGetValue(address, out var capability) is false)
            {
                return null;
            }

            lock (_solaxDataLock)
            {
                if (_knxValueBuffer.TryGetValue(capability, out var knxSolaxValue))
                {
                    return knxSolaxValue;
                }
            }

            return null;
        }
    }
}
