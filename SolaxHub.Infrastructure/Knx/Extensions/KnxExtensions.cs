using SolaxHub.Domain.Inverter;
using SolaxHub.Infrastructure.Knx.Options;

namespace SolaxHub.Infrastructure.Knx.Extensions;

internal static class KnxExtensions
{
    public static IEnumerable<KeyValuePair<string, string>> GetReadGroupAddressesFromOptions(this KnxOptions options)
        => options.ReadGroupAddresses.Where(m => string.IsNullOrEmpty(m.Value) is false);

    public static IEnumerable<KeyValuePair<string, string>> GetWriteGroupAddressesFromOptions(this KnxOptions options)
        => options.WriteGroupAddresses.Where(m => string.IsNullOrEmpty(m.Value) is false);

    public static int ToNormalizedLockState(this LockState state) => state switch
    {
        LockState.Unlocked => 1,
        LockState.UnlockedAdvanced => 2,
        _ => 0
    };
}
