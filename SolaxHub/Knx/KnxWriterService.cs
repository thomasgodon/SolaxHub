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
        private readonly Dictionary<string, KnxValue> _knxSolaxValueBuffer;
        private readonly Dictionary<GroupAddress, string> _capabilityAddressMapping;
        private readonly object _solaxDataLock = new();

        public bool Enabled => _options.Enabled;

        public KnxWriterService(IOptions<KnxOptions> options, IKnxClient knxClient)
        {
            _options = options.Value;
            _knxSolaxValueBuffer = BuildKnxSolaxValueBuffer(_options);
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
            if (_knxSolaxValueBuffer.TryGetValue(capability, out var knxSolaxValue) is false)
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

            _knxSolaxValueBuffer[capability].Value = value;
            _knxSolaxValueBuffer[capability].IsShort = isShort;
            return _knxSolaxValueBuffer[capability];
        }

        private IEnumerable<KnxValue?> UpdateValues(SolaxData solaxData)
        {
            lock (_solaxDataLock)
            {
                // HouseLoad - 14.056 power
                yield return UpdateValue(nameof(SolaxData.), BitConverter.GetBytes((float)solaxData.HouseLoad));
                // AcPower - 14.056 power
                yield return UpdateValue(nameof(SolaxData.AcPower), BitConverter.GetBytes((float)solaxData.AcPower));
                // BatteryPower - 14.056 power
                yield return UpdateValue(nameof(SolaxData.BatteryPower), BitConverter.GetBytes((float)solaxData.BatteryPower));
                // InverterUseMode - 6.020 status with mode
                yield return UpdateValue(nameof(SolaxData.InverterUseMode), new[] { (byte)((int)solaxData.InverterUseMode * 2.55) });
                // ConsumeEnergy - 14.* 4byte float value
                yield return UpdateValue(nameof(SolaxData.ConsumeEnergy), BitConverter.GetBytes((float)solaxData.ConsumeEnergy));
                // Soc - 5.001 percentage
                yield return UpdateValue(nameof(SolaxData.Soc), new[] { (byte)(solaxData.Soc * 2.55) });
                // EpsPowerR - 14.056 power
                yield return UpdateValue(nameof(SolaxData.EpsPowerR), BitConverter.GetBytes((float)(solaxData.EpsPowerR ?? 0)));
                // InverterStatus - 6.020 status with mode
                yield return UpdateValue(nameof(SolaxData.), new[] { (byte)solaxData.InverterStatus });
                // YieldToday - 14
                yield return UpdateValue(nameof(SolaxData.YieldToday), BitConverter.GetBytes((float)solaxData.YieldToday));
                // YieldTotal - 14
                yield return UpdateValue(nameof(SolaxData.YieldTotal), BitConverter.GetBytes((float)solaxData.YieldTotal));
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
                if (_knxSolaxValueBuffer.TryGetValue(capability, out var knxSolaxValue))
                {
                    return knxSolaxValue;
                }
            }

            return null;
        }
    }
}
