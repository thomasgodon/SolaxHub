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

    public KnxValue? UpdateMaxGridImportWatts(int watts)
    {
        lock (_mappingLock)
            return UpdateValue("MaxGridImportWatts", BitConverter.GetBytes((float)watts));
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
        yield return UpdateValue("ConsumeEnergy", BitConverter.GetBytes((float)inverter.Grid.ConsumeEnergy));
        yield return UpdateValue("BatteryCapacity", [(byte)(inverter.Battery.Capacity * 2.55)]);
        yield return UpdateValue("PvPower1", BitConverter.GetBytes((float)inverter.Solar.Power1));
        yield return UpdateValue("InverterStatus", [(byte)inverter.Status]);
        yield return UpdateValue("SolarEnergyToday", BitConverter.GetBytes((float)inverter.Solar.EnergyToday));
        yield return UpdateValue("SolarEnergyTotal", BitConverter.GetBytes((float)inverter.Solar.EnergyTotal));
        yield return UpdateValue("BatteryOutputEnergyToday", BitConverter.GetBytes((float)inverter.Battery.OutputToday));
        yield return UpdateValue("BatteryInputEnergyToday", BitConverter.GetBytes((float)inverter.Battery.InputToday));
        yield return UpdateValue("BatteryOutputEnergyTotal", BitConverter.GetBytes((float)inverter.Battery.OutputTotal));
        yield return UpdateValue("BatteryInputEnergyTotal", BitConverter.GetBytes((float)inverter.Battery.InputTotal));
        yield return UpdateValue("PowerControl", [(byte)(int)inverter.PowerControlMode]);
        yield return UpdateValue("LockState", [(byte)inverter.LockState.ToNormalizedLockState()]);
    }
}
