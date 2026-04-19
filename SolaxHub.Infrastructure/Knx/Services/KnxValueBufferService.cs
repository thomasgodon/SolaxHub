using Microsoft.Extensions.Options;
using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Knx.Extensions;
using SolaxHub.Infrastructure.Knx.Models;
using SolaxHub.Infrastructure.Knx.Options;

namespace SolaxHub.Infrastructure.Knx.Services;

internal class KnxValueBufferService : IKnxValueBufferService
{
    private readonly Dictionary<string, KnxValue> _capabilityKnxValueMapping;
    private readonly Lock _mappingLock = new();

    public KnxValueBufferService(IOptions<KnxOptions> options)
    {
        _capabilityKnxValueMapping = BuildCapabilityKnxValueMapping(options.Value);
    }

    public IEnumerable<KnxValue> UpdateKnxValues(Inverter inverter)
    {
        lock (_mappingLock)
        {
            return UpdateValues(inverter).Where(m => m is not null).ToList()!;
        }
    }

    public IReadOnlyDictionary<string, KnxValue> GetKnxValues()
    {
        lock (_mappingLock)
        {
            return _capabilityKnxValueMapping.ToDictionary(k => k.Key, v => v.Value);
        }
    }

    private KnxValue? UpdateValue(string capability, byte[] value)
    {
        if (_capabilityKnxValueMapping.TryGetValue(capability, out KnxValue? knxValue) is false)
            return null;

        if (knxValue.Value is not null && knxValue.Value.SequenceEqual(value))
            return null;

        _capabilityKnxValueMapping[capability].Value = value;
        return _capabilityKnxValueMapping[capability];
    }

    private static Dictionary<string, KnxValue> BuildCapabilityKnxValueMapping(KnxOptions options)
    {
        var mapping = new Dictionary<string, KnxValue>(options.ReadGroupAddresses.Count);
        foreach (var (key, address) in options.GetReadGroupAddressesFromOptions())
            mapping.Add(key, new KnxValue(address));
        return mapping;
    }

    private IEnumerable<KnxValue?> UpdateValues(Inverter inverter)
    {
        yield return UpdateValue("HouseLoad", BitConverter.GetBytes((float)inverter.HouseLoad));
        yield return UpdateValue("InverterPower", BitConverter.GetBytes((float)inverter.InverterPower));
        yield return UpdateValue("BatteryPower", BitConverter.GetBytes((float)inverter.Battery.Power));
        yield return UpdateValue("InverterUseMode", [(byte)(int)inverter.UseMode]);
        yield return UpdateValue("ConsumeEnergy", BitConverter.GetBytes((int)Math.Round(inverter.Grid.ConsumeEnergy * 1000.0)));
        yield return UpdateValue("BatteryCapacity", [(byte)(inverter.Battery.Capacity * 2.55)]);
        yield return UpdateValue("PvPower1", BitConverter.GetBytes((float)inverter.Solar.Power1));
        yield return UpdateValue("InverterStatus", [(byte)inverter.Status]);
        yield return UpdateValue("SolarEnergyToday", BitConverter.GetBytes((int)Math.Round(inverter.Solar.EnergyToday * 1000.0)));
        yield return UpdateValue("SolarEnergyTotal", BitConverter.GetBytes((int)Math.Round(inverter.Solar.EnergyTotal * 1000.0)));
        yield return UpdateValue("BatteryOutputEnergyToday", BitConverter.GetBytes((int)Math.Round(inverter.Battery.OutputToday * 1000.0)));
        yield return UpdateValue("BatteryInputEnergyToday", BitConverter.GetBytes((int)Math.Round(inverter.Battery.InputToday * 1000.0)));
        yield return UpdateValue("BatteryOutputEnergyTotal", BitConverter.GetBytes((int)Math.Round(inverter.Battery.OutputTotal * 1000.0)));
        yield return UpdateValue("BatteryInputEnergyTotal", BitConverter.GetBytes((int)Math.Round(inverter.Battery.InputTotal * 1000.0)));
        yield return UpdateValue("PowerControlMode", [(byte)(int)inverter.PowerControlMode]);
        yield return UpdateValue("LockState", [(byte)inverter.LockState.ToNormalizedLockState()]);
    }
}
