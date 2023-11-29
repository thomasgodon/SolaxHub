﻿using Microsoft.Extensions.Options;
using SolaxHub.Solax;
using SolaxHub.Knx.Client;

namespace SolaxHub.Knx
{
    internal class KnxSolaxProcessor : ISolaxProcessor
    {
        private readonly KnxOptions _options; 
        private readonly IKnxClient _knxClient;
        private readonly Dictionary<string, KnxSolaxValue> _knxSolaxValueBuffer;
        private readonly object _solaxDataLock = new();

        public KnxSolaxProcessor(IOptions<KnxOptions> options, IKnxClient knxClient)
        {
            _options = options.Value;
            _knxSolaxValueBuffer = BuildKnxSolaxValueBuffer(_options);
            _knxClient = knxClient;
        }

        public async Task ProcessData(SolaxData data, CancellationToken cancellationToken)
        {
            if (_options.Enabled is false) return;

            // get updated values
            var updatedValues = UpdateValues(data)
                .Where(m => m is not null).ToList() as IEnumerable<KnxSolaxValue>;

            await _knxClient.SendValuesAsync(updatedValues, cancellationToken);
        }

        private KnxSolaxValue? UpdateValue(string capability, byte[] value, bool isShort = false)
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

        private IEnumerable<KnxSolaxValue?> UpdateValues(SolaxData solaxData)
        {
            lock (_solaxDataLock)
            {
                // HouseLoad - 14.056 power
                yield return UpdateValue(nameof(SolaxData.HouseLoad), BitConverter.GetBytes((float)solaxData.HouseLoad));
                // AcPower - 14.056 power
                yield return UpdateValue(nameof(SolaxData.AcPower), BitConverter.GetBytes((float)solaxData.AcPower));
                // BatteryPower - 14.056 power
                yield return UpdateValue(nameof(SolaxData.BatteryPower), BitConverter.GetBytes((float)solaxData.BatteryPower));
                // BatteryStatus - 7.* 2-byte unsigned value
                yield return UpdateValue(nameof(SolaxData.BatteryStatus), BitConverter.GetBytes((ushort)solaxData.BatteryStatus));
                // ConsumeEnergy - 14.* 4byte float value
                yield return UpdateValue(nameof(SolaxData.ConsumeEnergy), BitConverter.GetBytes((float)solaxData.ConsumeEnergy));
                // ConsumeEnergy - 5.001 percentage
                yield return UpdateValue(nameof(SolaxData.Soc), BitConverter.GetBytes((byte)solaxData.Soc), isShort: true);
            }
        }

        private static Dictionary<string, KnxSolaxValue> BuildKnxSolaxValueBuffer(KnxOptions knxOptions)
        {
            var solaxData = new Dictionary<string, KnxSolaxValue>(knxOptions.GroupAddressMapping.Count);

            foreach (var groupAddressMapping in GroupAddressMappingsFromOptions(knxOptions))
            {
                solaxData.Add(groupAddressMapping.Key, new KnxSolaxValue(groupAddressMapping.Value));
            }

            return solaxData;
        }

        private static IEnumerable<KeyValuePair<string, string>> GroupAddressMappingsFromOptions(KnxOptions options)
            => options.GroupAddressMapping
                .Where(
                    mapping => string.IsNullOrEmpty(mapping.Value) is false);
    }
}
